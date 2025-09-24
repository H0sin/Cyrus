using System.Collections.Concurrent;
using System.Reflection;
using Cyrus.Core.Contracts.Plugins;
using Cyrus.Utilities.Guards.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cyrus.Core.Plugins;

/// <summary>
/// Default implementation of the plugin manager.
/// </summary>
public class PluginManager : IPluginManager, IDisposable
{
    private readonly ConcurrentDictionary<string, IPlugin> _plugins = new();
    private readonly ILogger<PluginManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;

    public PluginManager(ILogger<PluginManager> logger, IServiceProvider serviceProvider)
    {
        _logger = Against.Null(logger, nameof(logger));
        _serviceProvider = Against.Null(serviceProvider, nameof(serviceProvider));
    }

    /// <inheritdoc />
    public IReadOnlyList<IPlugin> Plugins => _plugins.Values.ToList();

    /// <inheritdoc />
    public IReadOnlyList<IPlugin> RunningPlugins => _plugins.Values.Where(p => p.IsRunning).ToList();

    /// <inheritdoc />
    public async Task DiscoverPluginsAsync(string pluginDirectory)
    {
        Against.NullOrWhiteSpace(pluginDirectory, nameof(pluginDirectory));

        if (!Directory.Exists(pluginDirectory))
        {
            _logger.LogWarning("Plugin directory does not exist: {PluginDirectory}", pluginDirectory);
            return;
        }

        _logger.LogInformation("Discovering plugins in directory: {PluginDirectory}", pluginDirectory);

        var assemblyFiles = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);
        
        foreach (var assemblyFile in assemblyFiles)
        {
            try
            {
                var plugin = await LoadPluginAsync(assemblyFile);
                await RegisterPluginAsync(plugin);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load plugin from assembly: {AssemblyPath}", assemblyFile);
            }
        }

        _logger.LogInformation("Plugin discovery completed. Found {PluginCount} plugins", _plugins.Count);
    }

    /// <inheritdoc />
    public async Task<IPlugin> LoadPluginAsync(string assemblyPath)
    {
        Against.NullOrWhiteSpace(assemblyPath, nameof(assemblyPath));

        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException($"Plugin assembly not found: {assemblyPath}");

        _logger.LogDebug("Loading plugin from assembly: {AssemblyPath}", assemblyPath);

        try
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            var pluginType = FindPluginType(assembly);

            if (pluginType == null)
                throw new InvalidOperationException($"No plugin implementation found in assembly: {assemblyPath}");

            var plugin = Activator.CreateInstance(pluginType) as IPlugin;
            if (plugin == null)
                throw new InvalidOperationException($"Failed to create plugin instance from type: {pluginType.FullName}");

            _logger.LogDebug("Successfully loaded plugin: {PluginName} (ID: {PluginId})", plugin.Metadata.Name, plugin.Metadata.Id);

            return plugin;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load plugin from assembly: {AssemblyPath}", assemblyPath);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RegisterPluginAsync(IPlugin plugin)
    {
        Against.Null(plugin, nameof(plugin));

        if (_plugins.ContainsKey(plugin.Metadata.Id))
        {
            _logger.LogWarning("Plugin with ID {PluginId} is already registered", plugin.Metadata.Id);
            return;
        }

        _logger.LogInformation("Registering plugin: {PluginName} (ID: {PluginId})", plugin.Metadata.Name, plugin.Metadata.Id);

        // Check dependencies
        await ValidatePluginDependencies(plugin);

        // Initialize the plugin with a temporary service collection
        // Note: In a real application, you'd want to rebuild the service provider
        // with the plugin services included. For this demo, we'll store the plugin
        // and register its services when starting.
        var tempServices = new ServiceCollection();
        await plugin.InitializeAsync(tempServices);

        // Register the plugin
        _plugins.TryAdd(plugin.Metadata.Id, plugin);

        _logger.LogInformation("Plugin registered successfully: {PluginName}", plugin.Metadata.Name);
    }

    /// <inheritdoc />
    public async Task UnregisterPluginAsync(string pluginId)
    {
        Against.NullOrWhiteSpace(pluginId, nameof(pluginId));

        if (!_plugins.TryRemove(pluginId, out var plugin))
        {
            _logger.LogWarning("Plugin with ID {PluginId} is not registered", pluginId);
            return;
        }

        _logger.LogInformation("Unregistering plugin: {PluginName} (ID: {PluginId})", plugin.Metadata.Name, plugin.Metadata.Id);

        try
        {
            if (plugin.IsRunning)
                await plugin.StopAsync();

            plugin.Dispose();
            _logger.LogInformation("Plugin unregistered successfully: {PluginName}", plugin.Metadata.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while unregistering plugin: {PluginName}", plugin.Metadata.Name);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task StartPluginAsync(string pluginId)
    {
        Against.NullOrWhiteSpace(pluginId, nameof(pluginId));

        var plugin = await GetPluginAsync(pluginId);
        if (plugin == null)
        {
            _logger.LogWarning("Plugin with ID {PluginId} not found", pluginId);
            return;
        }

        if (plugin.IsRunning)
        {
            _logger.LogDebug("Plugin {PluginName} is already running", plugin.Metadata.Name);
            return;
        }

        await plugin.StartAsync(_serviceProvider);
    }

    /// <inheritdoc />
    public async Task StopPluginAsync(string pluginId)
    {
        Against.NullOrWhiteSpace(pluginId, nameof(pluginId));

        var plugin = await GetPluginAsync(pluginId);
        if (plugin == null)
        {
            _logger.LogWarning("Plugin with ID {PluginId} not found", pluginId);
            return;
        }

        if (!plugin.IsRunning)
        {
            _logger.LogDebug("Plugin {PluginName} is not running", plugin.Metadata.Name);
            return;
        }

        await plugin.StopAsync();
    }

    /// <inheritdoc />
    public Task<IPlugin?> GetPluginAsync(string pluginId)
    {
        Against.NullOrWhiteSpace(pluginId, nameof(pluginId));

        _plugins.TryGetValue(pluginId, out var plugin);
        return Task.FromResult(plugin);
    }

    /// <inheritdoc />
    public async Task StartAllPluginsAsync()
    {
        _logger.LogInformation("Starting all registered plugins");

        var startTasks = _plugins.Values
            .Where(p => !p.IsRunning && p.Metadata.IsEnabledByDefault)
            .Select(p => StartPluginAsync(p.Metadata.Id));

        await Task.WhenAll(startTasks);

        _logger.LogInformation("All plugins started successfully");
    }

    /// <inheritdoc />
    public async Task StopAllPluginsAsync()
    {
        _logger.LogInformation("Stopping all running plugins");

        var stopTasks = RunningPlugins.Select(p => StopPluginAsync(p.Metadata.Id));
        await Task.WhenAll(stopTasks);

        _logger.LogInformation("All plugins stopped successfully");
    }

    private static Type? FindPluginType(Assembly assembly)
    {
        return assembly.GetTypes()
            .FirstOrDefault(t => !t.IsAbstract && !t.IsInterface && typeof(IPlugin).IsAssignableFrom(t));
    }

    private async Task ValidatePluginDependencies(IPlugin plugin)
    {
        foreach (var dependency in plugin.Metadata.Dependencies)
        {
            var dependentPlugin = await GetPluginAsync(dependency);
            if (dependentPlugin == null)
            {
                throw new InvalidOperationException(
                    $"Plugin {plugin.Metadata.Name} requires dependency {dependency} which is not registered");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        try
        {
            StopAllPluginsAsync().GetAwaiter().GetResult();

            foreach (var plugin in _plugins.Values)
            {
                plugin.Dispose();
            }

            _plugins.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while disposing plugin manager");
        }
        finally
        {
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}