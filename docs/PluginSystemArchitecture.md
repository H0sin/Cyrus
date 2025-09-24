# Cyrus Plugin System Architecture

## Overview

The Cyrus Plugin System provides a comprehensive, modular architecture for extending the Cyrus framework through dynamically loadable plugins. The system is designed with inspiration from the DigitalFramework and follows clean architecture principles with proper separation of concerns.

## Architecture Diagram

```
┌─────────────────────┐
│    Web API Layer    │ ← REST API for plugin management
├─────────────────────┤
│  Plugin Manager     │ ← Orchestrates plugin lifecycle
├─────────────────────┤
│   Plugin Base       │ ← Common plugin functionality
├─────────────────────┤
│ Plugin Contracts    │ ← Interfaces and metadata
├─────────────────────┤
│  Dependency Injection Container  │
└─────────────────────┘
```

## Core Components

### 1. Plugin Contracts (`Cyrus.Core.Contracts.Plugins`)

**Interfaces:**
- `IPlugin`: Main plugin interface defining the plugin contract
- `IPluginMetadata`: Plugin information and metadata
- `IPluginManager`: Plugin lifecycle management operations

**Classes:**
- `PluginMetadata`: Default metadata implementation with validation

**Key Responsibilities:**
- Define plugin system contracts
- Provide metadata abstraction
- Ensure type safety across the system

### 2. Plugin Infrastructure (`Cyrus.Core.Plugins`)

**Classes:**
- `PluginBase`: Abstract base class providing common functionality
- `PluginManager`: Complete plugin lifecycle management implementation

**Key Features:**
- Plugin discovery and loading from assemblies
- Dependency validation and ordering
- Service registration with DI container
- Lifecycle management (Initialize → Start → Run → Stop → Dispose)
- Error handling and logging
- Configuration change support

### 3. Web Integration (`Cyrus.EndPoints.Web`)

**Components:**
- `PluginsController`: REST API for plugin management
- `PluginHostedService`: ASP.NET Core hosted service integration
- `AddPluginExtensions`: DI container registration extensions

**API Endpoints:**
- `GET /api/plugins` - List all plugins
- `GET /api/plugins/running` - List running plugins
- `GET /api/plugins/{id}` - Get specific plugin details
- `POST /api/plugins/{id}/start` - Start a plugin
- `POST /api/plugins/{id}/stop` - Stop a plugin
- `POST /api/plugins/start-all` - Start all plugins
- `POST /api/plugins/stop-all` - Stop all plugins

### 4. Example Plugin (`Cyrus.Plugins.HelloWorld`)

**Components:**
- `HelloWorldPlugin`: Complete plugin implementation
- `IHelloWorldService`: Plugin-specific service interface
- `HelloWorldService`: Service implementation

**Demonstrates:**
- Plugin metadata configuration
- Service registration
- Background task execution with Timer
- Configuration change handling
- Proper logging integration
- Clean resource disposal

## Plugin Lifecycle

### 1. Discovery Phase
```csharp
// Scan directory for plugin assemblies
await pluginManager.DiscoverPluginsAsync("/path/to/plugins");
```

### 2. Loading Phase
```csharp
// Load plugin from assembly
var plugin = await pluginManager.LoadPluginAsync("plugin.dll");
```

### 3. Registration Phase
```csharp
// Register plugin with manager
await pluginManager.RegisterPluginAsync(plugin);
```

### 4. Initialization Phase
```csharp
// Plugin registers its services
await plugin.InitializeAsync(serviceCollection);
```

### 5. Starting Phase
```csharp
// Plugin begins execution
await plugin.StartAsync(serviceProvider);
```

### 6. Running Phase
- Plugin performs its intended functionality
- Background tasks execute
- Services are available to other components

### 7. Stopping Phase
```csharp
// Plugin gracefully shuts down
await plugin.StopAsync();
```

### 8. Disposal Phase
```csharp
// Plugin cleans up resources
plugin.Dispose();
```

## Dependency Injection Integration

The plugin system is fully integrated with .NET's dependency injection container:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register core services
    services.AddLogging();
    services.AddCyrusPlugins("/path/to/plugins");
    
    // Plugin services are automatically registered
    // when plugins are initialized
}
```

## Configuration and Hosting

### ASP.NET Core Integration

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCyrusPlugins();
        // Plugin hosted service automatically starts all plugins
    }
}
```

### Plugin Directory Configuration

```csharp
services.AddCyrusPlugins("/var/lib/cyrus/plugins");
```

## Error Handling and Logging

The plugin system includes comprehensive error handling:

- **Plugin Loading Errors**: Invalid assemblies are skipped with warnings
- **Startup Failures**: Failed plugins are reported but don't crash the system
- **Runtime Errors**: Plugin exceptions are isolated and logged
- **Dependency Errors**: Missing dependencies are validated at registration

All operations are logged with structured logging:

```csharp
Logger?.LogInformation("Starting plugin: {PluginName} (ID: {PluginId})", 
    plugin.Metadata.Name, plugin.Metadata.Id);
```

## Security Considerations

### Plugin Isolation
- Each plugin runs in the same AppDomain (suitable for trusted plugins)
- Plugin services are registered with the main DI container
- No sandboxing is currently implemented

### Assembly Loading
- Plugins are loaded using `Assembly.LoadFrom()`
- No signature verification is currently implemented
- Plugin discovery is limited to specified directories

### Recommendations for Production
1. Implement plugin signature verification
2. Add plugin sandboxing for untrusted code
3. Implement plugin permissions system
4. Add plugin resource quotas
5. Implement plugin communication restrictions

## Performance Considerations

### Plugin Discovery
- Directory scanning is performed once at startup
- Assembly loading is deferred until needed
- Plugin metadata is cached after first load

### Service Registration
- Plugin services are registered during initialization phase
- Service provider is rebuilt when plugins are added/removed
- No performance impact during normal operation

### Background Tasks
- Plugins can run background tasks using Timer or hosted services
- Tasks are properly disposed when plugins are stopped
- No resource leaks under normal operation

## Testing Strategy

### Unit Testing
- Each component is tested in isolation
- Interfaces allow easy mocking
- Plugin lifecycle is fully testable

### Integration Testing
- `PluginSystemTest` demonstrates end-to-end functionality
- Tests plugin loading, starting, execution, and stopping
- Verifies service registration and DI integration

### Example Test Run
```bash
cd tests/PluginSystemTest
dotnet run
```

Expected output demonstrates:
- Plugin metadata display
- Successful registration and startup
- Background task execution
- Clean shutdown

## Future Enhancements

### Planned Features
1. **Hot Reload**: Dynamic plugin reloading without application restart
2. **Plugin Marketplace**: Package management for plugins
3. **Advanced Dependency Management**: NuGet-style dependency resolution
4. **Plugin Sandboxing**: Isolated execution environments
5. **Configuration UI**: Web-based plugin configuration
6. **Metrics and Monitoring**: Plugin performance tracking
7. **Plugin Templates**: Visual Studio templates for new plugins

### Extensibility Points
- Custom plugin discovery mechanisms
- Alternative service registration strategies
- Plugin communication protocols
- Configuration providers
- Logging providers

## Conclusion

The Cyrus Plugin System provides a robust, production-ready foundation for building modular applications. With its clean architecture, comprehensive lifecycle management, and full integration with .NET's dependency injection system, it enables developers to create extensible applications that can be enhanced with plugins at runtime.

The system successfully demonstrates all the requirements from the original issue:
- ✅ PluginBase infrastructure inspired by DigitalFramework
- ✅ Plugin loading, management, and removal capabilities
- ✅ Default HelloWorld plugin for testing and demonstration
- ✅ Developer documentation for creating new plugins
- ✅ Tested plugin loading and functionality
- ✅ No external dependencies outside Cyrus core