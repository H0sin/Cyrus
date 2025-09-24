namespace Cyrus.Core.Contracts.Plugins;

/// <summary>
/// Represents metadata information about a plugin.
/// </summary>
public interface IPluginMetadata
{
    /// <summary>
    /// Gets the unique identifier of the plugin.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the plugin.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the author of the plugin.
    /// </summary>
    string Author { get; }

    /// <summary>
    /// Gets the list of dependencies required by this plugin.
    /// </summary>
    IReadOnlyList<string> Dependencies { get; }

    /// <summary>
    /// Gets whether the plugin is enabled by default.
    /// </summary>
    bool IsEnabledByDefault { get; }
}