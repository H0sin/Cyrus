# Cyrus Plugin System

This project contains the core plugin infrastructure for the Cyrus framework, providing a modular architecture inspired by DigitalFramework.

## Overview

The Cyrus Plugin System enables dynamic loading and management of modular components at runtime. It provides a robust foundation for building extensible applications with proper lifecycle management, dependency injection integration, and comprehensive plugin management capabilities.

## Core Components

### Interfaces
- **`IPlugin`**: Main interface all plugins must implement
- **`IPluginMetadata`**: Provides plugin information and metadata
- **`IPluginManager`**: Manages plugin lifecycle operations

### Classes
- **`PluginBase`**: Abstract base class providing common plugin functionality
- **`PluginManager`**: Complete implementation of plugin lifecycle management
- **`PluginMetadata`**: Default implementation of plugin metadata

## Quick Start

### 1. Create a Plugin

```csharp
public class MyPlugin : PluginBase
{
    private readonly IPluginMetadata _metadata;

    public MyPlugin()
    {
        _metadata = new PluginMetadata(
            id: "my-plugin",
            name: "My Plugin",
            description: "A sample plugin",
            version: "1.0.0",
            author: "Your Name"
        );
    }

    public override IPluginMetadata Metadata => _metadata;

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IMyService, MyService>();
    }

    protected override Task OnStartAsync(IServiceProvider serviceProvider)
    {
        Logger?.LogInformation("My plugin started!");
        return Task.CompletedTask;
    }
}
```

### 2. Register Plugin System

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCyrusPlugins("/path/to/plugins");
}
```

### 3. Use Plugin Management API

```bash
# Get all plugins
GET /api/plugins

# Start a plugin
POST /api/plugins/my-plugin/start

# Stop a plugin
POST /api/plugins/my-plugin/stop
```

## Features

✅ **Dynamic Plugin Loading**: Load plugins from assemblies at runtime  
✅ **Lifecycle Management**: Initialize → Start → Run → Stop → Dispose  
✅ **Dependency Injection**: Full integration with .NET DI container  
✅ **Service Registration**: Plugins can register their own services  
✅ **Dependency Management**: Plugin dependencies validation and ordering  
✅ **Configuration Support**: Handle configuration changes at runtime  
✅ **REST API**: Complete HTTP API for plugin management  
✅ **Logging Integration**: Structured logging throughout plugin lifecycle  
✅ **Error Handling**: Robust error handling and recovery  
✅ **Background Tasks**: Support for long-running background operations  

## Example Plugin

See `Cyrus.Plugins.HelloWorld` for a complete example demonstrating:
- Plugin structure and metadata
- Service registration
- Background tasks with Timer
- Configuration handling
- Proper logging
- Clean shutdown

## Documentation

For detailed documentation, see:
- [Plugin Development Guide](../../../docs/PluginDevelopmentGuide.md)
- [API Reference](../../../docs/PluginDevelopmentGuide.md#plugin-management-api)

## Testing

Run the plugin system test:

```bash
cd tests/PluginSystemTest
dotnet run
```

This will demonstrate plugin loading, starting, execution, and shutdown.