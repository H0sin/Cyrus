using Microsoft.Extensions.DependencyInjection;

namespace Cyrus.Core.Contracts.Plugins;

/// <summary>
/// Represents the base interface for all plugins in the Cyrus system.
/// </summary>
public interface IPlugin : IDisposable
{
    /// <summary>
    /// Gets the metadata for this plugin.
    /// </summary>
    IPluginMetadata Metadata { get; }

    /// <summary>
    /// Gets a value indicating whether the plugin is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Initializes the plugin with the provided service collection.
    /// This method is called during the plugin registration phase.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitializeAsync(IServiceCollection services);

    /// <summary>
    /// Starts the plugin. This method is called after all plugins have been initialized.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency resolution.</param>
    /// <returns>A task representing the asynchronous start operation.</returns>
    Task StartAsync(IServiceProvider serviceProvider);

    /// <summary>
    /// Stops the plugin. This method is called when the plugin needs to be stopped.
    /// </summary>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    Task StopAsync();

    /// <summary>
    /// Called when the plugin configuration has changed.
    /// </summary>
    /// <param name="configuration">The new configuration data.</param>
    /// <returns>A task representing the asynchronous configuration update operation.</returns>
    Task OnConfigurationChangedAsync(IDictionary<string, object> configuration);
}