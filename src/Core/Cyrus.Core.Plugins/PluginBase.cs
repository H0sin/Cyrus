using Cyrus.Core.Contracts.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cyrus.Core.Plugins;

/// <summary>
/// Base class for plugins that provides common functionality and lifecycle management.
/// </summary>
public abstract class PluginBase : IPlugin
{
    private bool _disposed;
    protected ILogger? Logger { get; private set; }

    /// <inheritdoc />
    public abstract IPluginMetadata Metadata { get; }

    /// <inheritdoc />
    public bool IsRunning { get; private set; }

    /// <inheritdoc />
    public virtual Task InitializeAsync(IServiceCollection services)
    {
        RegisterServices(services);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task StartAsync(IServiceProvider serviceProvider)
    {
        if (IsRunning)
            return;

        Logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType());
        Logger?.LogInformation("Starting plugin: {PluginName} (ID: {PluginId})", Metadata.Name, Metadata.Id);

        try
        {
            await OnStartAsync(serviceProvider);
            IsRunning = true;
            Logger?.LogInformation("Plugin started successfully: {PluginName}", Metadata.Name);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to start plugin: {PluginName}", Metadata.Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task StopAsync()
    {
        if (!IsRunning)
            return;

        Logger?.LogInformation("Stopping plugin: {PluginName} (ID: {PluginId})", Metadata.Name, Metadata.Id);

        try
        {
            await OnStopAsync();
            IsRunning = false;
            Logger?.LogInformation("Plugin stopped successfully: {PluginName}", Metadata.Name);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error occurred while stopping plugin: {PluginName}", Metadata.Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual Task OnConfigurationChangedAsync(IDictionary<string, object> configuration)
    {
        Logger?.LogDebug("Configuration changed for plugin: {PluginName}", Metadata.Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Register services specific to this plugin with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    protected virtual void RegisterServices(IServiceCollection services)
    {
        // Default implementation does nothing
    }

    /// <summary>
    /// Called when the plugin should start its operations.
    /// Override this method to implement plugin-specific startup logic.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency resolution.</param>
    /// <returns>A task representing the asynchronous start operation.</returns>
    protected virtual Task OnStartAsync(IServiceProvider serviceProvider)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the plugin should stop its operations.
    /// Override this method to implement plugin-specific shutdown logic.
    /// </summary>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    protected virtual Task OnStopAsync()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        try
        {
            if (IsRunning)
            {
                StopAsync().GetAwaiter().GetResult();
            }

            OnDispose();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error occurred while disposing plugin: {PluginName}", Metadata.Name);
        }
        finally
        {
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Called when the plugin is being disposed.
    /// Override this method to implement plugin-specific disposal logic.
    /// </summary>
    protected virtual void OnDispose()
    {
        // Default implementation does nothing
    }
}