namespace Cyrus.Core.Contracts.Plugins;

/// <summary>
/// Represents the plugin manager responsible for discovering, loading, and managing plugins.
/// </summary>
public interface IPluginManager
{
    /// <summary>
    /// Gets all registered plugins.
    /// </summary>
    IReadOnlyList<IPlugin> Plugins { get; }

    /// <summary>
    /// Gets all running plugins.
    /// </summary>
    IReadOnlyList<IPlugin> RunningPlugins { get; }

    /// <summary>
    /// Discovers plugins from the specified directory.
    /// </summary>
    /// <param name="pluginDirectory">The directory to search for plugins.</param>
    /// <returns>A task representing the asynchronous discovery operation.</returns>
    Task DiscoverPluginsAsync(string pluginDirectory);

    /// <summary>
    /// Loads a plugin from the specified assembly path.
    /// </summary>
    /// <param name="assemblyPath">The path to the plugin assembly.</param>
    /// <returns>A task returning the loaded plugin.</returns>
    Task<IPlugin> LoadPluginAsync(string assemblyPath);

    /// <summary>
    /// Registers a plugin with the manager.
    /// </summary>
    /// <param name="plugin">The plugin to register.</param>
    /// <returns>A task representing the asynchronous registration operation.</returns>
    Task RegisterPluginAsync(IPlugin plugin);

    /// <summary>
    /// Unregisters a plugin from the manager.
    /// </summary>
    /// <param name="pluginId">The ID of the plugin to unregister.</param>
    /// <returns>A task representing the asynchronous unregistration operation.</returns>
    Task UnregisterPluginAsync(string pluginId);

    /// <summary>
    /// Starts a plugin by its ID.
    /// </summary>
    /// <param name="pluginId">The ID of the plugin to start.</param>
    /// <returns>A task representing the asynchronous start operation.</returns>
    Task StartPluginAsync(string pluginId);

    /// <summary>
    /// Stops a plugin by its ID.
    /// </summary>
    /// <param name="pluginId">The ID of the plugin to stop.</param>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    Task StopPluginAsync(string pluginId);

    /// <summary>
    /// Gets a plugin by its ID.
    /// </summary>
    /// <param name="pluginId">The ID of the plugin to retrieve.</param>
    /// <returns>The plugin if found, otherwise null.</returns>
    Task<IPlugin?> GetPluginAsync(string pluginId);

    /// <summary>
    /// Starts all registered plugins.
    /// </summary>
    /// <returns>A task representing the asynchronous start operation.</returns>
    Task StartAllPluginsAsync();

    /// <summary>
    /// Stops all running plugins.
    /// </summary>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    Task StopAllPluginsAsync();
}