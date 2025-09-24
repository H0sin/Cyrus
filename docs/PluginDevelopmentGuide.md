# Cyrus Plugin Development Guide

This document provides a comprehensive guide for developing plugins for the Cyrus framework.

## Overview

The Cyrus Plugin System allows you to extend the functionality of the Cyrus framework by creating modular components that can be loaded, managed, and executed at runtime. The plugin system is inspired by the DigitalFramework architecture and provides a robust foundation for building extensible applications.

## Plugin Architecture

### Core Components

1. **IPlugin**: The main interface that all plugins must implement
2. **IPluginMetadata**: Provides metadata information about a plugin
3. **IPluginManager**: Manages the lifecycle of all plugins
4. **PluginBase**: Abstract base class that provides common plugin functionality

### Plugin Lifecycle

Plugins go through the following lifecycle phases:

1. **Discovery**: Plugins are discovered from specified directories
2. **Loading**: Plugin assemblies are loaded into memory
3. **Registration**: Plugins are registered with the plugin manager
4. **Initialization**: Plugin services are registered with the DI container
5. **Starting**: Plugins begin their execution
6. **Running**: Plugins perform their intended functionality
7. **Stopping**: Plugins are gracefully shut down
8. **Disposal**: Plugin resources are cleaned up

## Creating a Plugin

### Step 1: Create a New Class Library Project

```bash
dotnet new classlib -n MyPlugin
cd MyPlugin
dotnet add package Cyrus.Core.Plugins
```

### Step 2: Implement the Plugin Interface

```csharp
using Cyrus.Core.Contracts.Plugins;
using Cyrus.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace MyPlugin;

public class MyPlugin : PluginBase
{
    private readonly IPluginMetadata _metadata;

    public MyPlugin()
    {
        _metadata = new PluginMetadata(
            id: "my-plugin",
            name: "My Plugin",
            description: "A sample plugin for demonstration",
            version: "1.0.0",
            author: "Your Name",
            dependencies: new List<string>(), // List any plugin dependencies
            isEnabledByDefault: true
        );
    }

    public override IPluginMetadata Metadata => _metadata;

    protected override void RegisterServices(IServiceCollection services)
    {
        // Register your plugin-specific services here
        services.AddSingleton<IMyService, MyService>();
    }

    protected override Task OnStartAsync(IServiceProvider serviceProvider)
    {
        // Plugin startup logic goes here
        Logger?.LogInformation("My plugin is starting...");
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync()
    {
        // Plugin shutdown logic goes here
        Logger?.LogInformation("My plugin is stopping...");
        return Task.CompletedTask;
    }
}
```

### Step 3: Implement Your Plugin Services

```csharp
public interface IMyService
{
    Task DoSomethingAsync();
}

public class MyService : IMyService
{
    public async Task DoSomethingAsync()
    {
        // Your service implementation
        await Task.Delay(100);
    }
}
```

## Plugin Configuration

### Metadata Properties

- **Id**: Unique identifier for the plugin (required)
- **Name**: Human-readable name of the plugin (required)  
- **Description**: Brief description of plugin functionality (required)
- **Version**: Version string following semantic versioning (required)
- **Author**: Plugin author information (required)
- **Dependencies**: List of plugin IDs that this plugin depends on (optional)
- **IsEnabledByDefault**: Whether the plugin should start automatically (optional, default: true)

### Plugin Dependencies

If your plugin depends on other plugins, specify their IDs in the Dependencies list:

```csharp
dependencies: new List<string> { "core-services-plugin", "logging-plugin" }
```

The plugin manager will ensure dependencies are loaded and started before your plugin.

## Integration with Cyrus Framework

### Registering the Plugin System

Add the plugin system to your application's startup:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Other service registrations...
    
    services.AddCyrusPlugins("/path/to/plugins");
}
```

### Plugin Management API

The framework provides a REST API for managing plugins:

- `GET /api/plugins` - List all plugins
- `GET /api/plugins/running` - List running plugins
- `GET /api/plugins/{id}` - Get specific plugin details
- `POST /api/plugins/{id}/start` - Start a plugin
- `POST /api/plugins/{id}/stop` - Stop a plugin
- `POST /api/plugins/start-all` - Start all plugins
- `POST /api/plugins/stop-all` - Stop all plugins

## Best Practices

### 1. Use Dependency Injection

Register your services using the `RegisterServices` method:

```csharp
protected override void RegisterServices(IServiceCollection services)
{
    services.AddScoped<IMyService, MyService>();
    services.AddSingleton<IMyConfiguration, MyConfiguration>();
}
```

### 2. Handle Errors Gracefully

```csharp
protected override async Task OnStartAsync(IServiceProvider serviceProvider)
{
    try
    {
        // Your startup code
        await InitializeResourcesAsync();
        Logger?.LogInformation("Plugin started successfully");
    }
    catch (Exception ex)
    {
        Logger?.LogError(ex, "Failed to start plugin");
        throw; // Re-throw to signal startup failure
    }
}
```

### 3. Clean Up Resources

```csharp
protected override async Task OnStopAsync()
{
    try
    {
        // Clean up resources
        await _timer?.DisposeAsync();
        _httpClient?.Dispose();
    }
    catch (Exception ex)
    {
        Logger?.LogError(ex, "Error during plugin shutdown");
    }
}
```

### 4. Use Configuration

```csharp
public override Task OnConfigurationChangedAsync(IDictionary<string, object> configuration)
{
    if (configuration.TryGetValue("interval", out var intervalValue))
    {
        if (int.TryParse(intervalValue.ToString(), out var interval))
        {
            _updateInterval = TimeSpan.FromSeconds(interval);
            Logger?.LogInformation("Update interval changed to {Interval}", _updateInterval);
        }
    }
    
    return base.OnConfigurationChangedAsync(configuration);
}
```

## Example: HelloWorld Plugin

See the `Cyrus.Plugins.HelloWorld` project for a complete example of a plugin implementation. This plugin demonstrates:

- Basic plugin structure
- Service registration
- Periodic background tasks
- Configuration handling
- Proper logging

## Deployment

1. Build your plugin project
2. Copy the output assembly to the plugins directory
3. Restart the application or use the plugin management API to load the plugin dynamically

## Troubleshooting

### Common Issues

1. **Plugin Not Found**: Ensure the plugin assembly is in the correct directory and implements IPlugin
2. **Dependency Errors**: Check that all required dependencies are available and their versions are compatible
3. **Startup Failures**: Check the logs for detailed error information during plugin initialization

### Logging

All plugin operations are logged. Check the application logs for detailed information about plugin loading, starting, and any errors that occur.

The plugin system uses structured logging with the following log levels:
- **Information**: Plugin lifecycle events (start, stop, configuration changes)
- **Warning**: Non-critical issues (plugin not found, dependency warnings)
- **Error**: Critical failures (startup errors, exceptions)
- **Debug**: Detailed diagnostic information

## Future Enhancements

The plugin system is designed to be extensible. Future enhancements may include:

- Hot-reload capabilities for development
- Plugin sandboxing and security
- Plugin dependency resolution from package sources
- Plugin configuration UI
- Performance monitoring and metrics