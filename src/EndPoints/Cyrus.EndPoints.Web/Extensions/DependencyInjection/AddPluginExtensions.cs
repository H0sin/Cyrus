using Cyrus.Core.Contracts.Plugins;
using Cyrus.Core.Plugins;

namespace Cyrus.EndPoints.Web.Extensions.DependencyInjection;

/// <summary>
/// Extensions for adding plugin services to the dependency injection container.
/// </summary>
public static class AddPluginExtensions
{
    /// <summary>
    /// Adds plugin services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="pluginDirectory">Optional directory to discover plugins from.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCyrusPlugins(this IServiceCollection services, string? pluginDirectory = null)
    {
        // Register the plugin manager
        services.AddSingleton<IPluginManager, PluginManager>();

        // Register a hosted service to manage plugin lifecycle
        services.AddHostedService<PluginHostedService>();

        // Store plugin directory configuration if provided
        if (!string.IsNullOrWhiteSpace(pluginDirectory))
        {
            services.AddSingleton<PluginConfiguration>(new PluginConfiguration { PluginDirectory = pluginDirectory });
        }

        return services;
    }
}