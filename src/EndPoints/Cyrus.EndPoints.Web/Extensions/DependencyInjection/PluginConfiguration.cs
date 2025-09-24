namespace Cyrus.EndPoints.Web.Extensions.DependencyInjection;

/// <summary>
/// Configuration class for plugin settings.
/// </summary>
public class PluginConfiguration
{
    /// <summary>
    /// Gets or sets the directory where plugins are located.
    /// </summary>
    public string PluginDirectory { get; set; } = string.Empty;
}