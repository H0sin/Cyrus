using Cyrus.Core.Contracts.Plugins;
using Cyrus.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cyrus.Plugins.HelloWorld;

/// <summary>
/// A simple HelloWorld plugin that demonstrates the plugin system functionality.
/// </summary>
public class HelloWorldPlugin : PluginBase
{
    private readonly IPluginMetadata _metadata;
    private Timer? _timer;

    public HelloWorldPlugin()
    {
        _metadata = new PluginMetadata(
            id: "hello-world-plugin",
            name: "Hello World Plugin",
            description: "A simple plugin that demonstrates the Cyrus plugin system with periodic hello messages.",
            version: "1.0.0",
            author: "Cyrus Team",
            dependencies: new List<string>(),
            isEnabledByDefault: true
        );
    }

    /// <inheritdoc />
    public override IPluginMetadata Metadata => _metadata;

    /// <inheritdoc />
    protected override void RegisterServices(IServiceCollection services)
    {
        // Register plugin-specific services
        services.AddSingleton<IHelloWorldService, HelloWorldService>();
        Logger?.LogDebug("Registered HelloWorld services");
    }

    /// <inheritdoc />
    protected override Task OnStartAsync(IServiceProvider serviceProvider)
    {
        Logger?.LogInformation("HelloWorld plugin is starting...");

        var helloWorldService = serviceProvider.GetRequiredService<IHelloWorldService>();
        
        // Start a timer that will log a hello message every 30 seconds
        _timer = new Timer(async _ =>
        {
            var message = helloWorldService.GetGreeting();
            Logger?.LogInformation("HelloWorld Plugin says: {Message}", message);
            
            // Simulate some async work
            await Task.Delay(100);
        }, 
        null, 
        TimeSpan.FromSeconds(5), // Initial delay
        TimeSpan.FromSeconds(30)); // Repeat every 30 seconds

        Logger?.LogInformation("HelloWorld plugin started successfully!");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStopAsync()
    {
        Logger?.LogInformation("HelloWorld plugin is stopping...");

        _timer?.Dispose();
        _timer = null;

        Logger?.LogInformation("HelloWorld plugin stopped successfully!");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        _timer?.Dispose();
        base.OnDispose();
    }

    /// <inheritdoc />
    public override Task OnConfigurationChangedAsync(IDictionary<string, object> configuration)
    {
        Logger?.LogInformation("HelloWorld plugin configuration changed");
        
        if (configuration.TryGetValue("greeting", out var greeting))
        {
            Logger?.LogInformation("New greeting configuration: {Greeting}", greeting);
        }

        return base.OnConfigurationChangedAsync(configuration);
    }
}