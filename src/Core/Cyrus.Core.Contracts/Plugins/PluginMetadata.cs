using Cyrus.Utilities.Guards.GuardClauses;

namespace Cyrus.Core.Contracts.Plugins;

/// <summary>
/// Default implementation of plugin metadata.
/// </summary>
public class PluginMetadata : IPluginMetadata
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Version { get; }
    public string Author { get; }
    public IReadOnlyList<string> Dependencies { get; }
    public bool IsEnabledByDefault { get; }

    public PluginMetadata(
        string id,
        string name,
        string description,
        string version,
        string author,
        IReadOnlyList<string>? dependencies = null,
        bool isEnabledByDefault = true)
    {
        Id = Against.NullOrWhiteSpace(id, nameof(id));
        Name = Against.NullOrWhiteSpace(name, nameof(name));
        Description = Against.NullOrWhiteSpace(description, nameof(description));
        Version = Against.NullOrWhiteSpace(version, nameof(version));
        Author = Against.NullOrWhiteSpace(author, nameof(author));
        Dependencies = dependencies ?? new List<string>();
        IsEnabledByDefault = isEnabledByDefault;
    }
}