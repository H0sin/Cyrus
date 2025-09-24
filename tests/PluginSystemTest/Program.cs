using Cyrus.Core.Contracts.Plugins;
using Cyrus.Core.Plugins;
using Cyrus.Plugins.HelloWorld;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Setup logging and services including HelloWorld service directly
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
services.AddSingleton<Cyrus.Plugins.HelloWorld.IHelloWorldService, Cyrus.Plugins.HelloWorld.HelloWorldService>();

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

logger.LogInformation("Starting Plugin System Test");

try
{
    // Create plugin manager
    var pluginManagerLogger = loggerFactory.CreateLogger<PluginManager>();
    var pluginManager = new PluginManager(pluginManagerLogger, serviceProvider);

    // Create HelloWorld plugin
    var helloWorldPlugin = new HelloWorldPlugin();

    logger.LogInformation("Plugin Metadata:");
    logger.LogInformation("  ID: {Id}", helloWorldPlugin.Metadata.Id);
    logger.LogInformation("  Name: {Name}", helloWorldPlugin.Metadata.Name);
    logger.LogInformation("  Description: {Description}", helloWorldPlugin.Metadata.Description);
    logger.LogInformation("  Version: {Version}", helloWorldPlugin.Metadata.Version);
    logger.LogInformation("  Author: {Author}", helloWorldPlugin.Metadata.Author);

    // Register the plugin
    logger.LogInformation("Registering plugin...");
    await pluginManager.RegisterPluginAsync(helloWorldPlugin);

    // Start the plugin
    logger.LogInformation("Starting plugin...");
    await pluginManager.StartPluginAsync(helloWorldPlugin.Metadata.Id);

    // Verify plugin is running
    var runningPlugins = pluginManager.RunningPlugins;
    logger.LogInformation("Running plugins count: {Count}", runningPlugins.Count);

    if (runningPlugins.Any())
    {
        logger.LogInformation("Plugin is running successfully!");
        
        // Let it run for a bit to see the periodic messages
        logger.LogInformation("Waiting 12 seconds to observe plugin behavior...");
        await Task.Delay(12000);
    }

    // Stop the plugin
    logger.LogInformation("Stopping plugin...");
    await pluginManager.StopPluginAsync(helloWorldPlugin.Metadata.Id);

    // Verify plugin is stopped
    var runningPluginsAfterStop = pluginManager.RunningPlugins;
    logger.LogInformation("Running plugins count after stop: {Count}", runningPluginsAfterStop.Count);

    logger.LogInformation("Plugin System Test completed successfully!");
}
catch (Exception ex)
{
    logger.LogError(ex, "Plugin System Test failed");
    Environment.Exit(1);
}

Environment.Exit(0);