using Cyrus.Core.Contracts.Plugins;

namespace Cyrus.EndPoints.Web.Extensions.DependencyInjection;

/// <summary>
/// Hosted service that manages the plugin lifecycle.
/// </summary>
public class PluginHostedService : IHostedService
{
    private readonly IPluginManager _pluginManager;
    private readonly PluginConfiguration? _configuration;
    private readonly ILogger<PluginHostedService> _logger;

    public PluginHostedService(
        IPluginManager pluginManager,
        ILogger<PluginHostedService> logger,
        PluginConfiguration? configuration = null)
    {
        _pluginManager = pluginManager;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting plugin hosted service");

        try
        {
            // Discover plugins from directory if configured
            if (_configuration != null && !string.IsNullOrWhiteSpace(_configuration.PluginDirectory))
            {
                _logger.LogInformation("Discovering plugins from directory: {PluginDirectory}", _configuration.PluginDirectory);
                await _pluginManager.DiscoverPluginsAsync(_configuration.PluginDirectory);
            }

            // Start all plugins
            await _pluginManager.StartAllPluginsAsync();

            _logger.LogInformation("Plugin hosted service started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start plugin hosted service");
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping plugin hosted service");

        try
        {
            await _pluginManager.StopAllPluginsAsync();
            _logger.LogInformation("Plugin hosted service stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while stopping plugin hosted service");
        }
    }
}