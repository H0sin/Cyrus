using Cyrus.Core.Contracts.Plugins;
using Microsoft.AspNetCore.Mvc;

namespace Cyrus.EndPoints.Web.Controllers;

/// <summary>
/// Controller for managing plugins.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PluginsController : BaseController
{
    private readonly IPluginManager _pluginManager;
    private readonly ILogger<PluginsController> _logger;

    public PluginsController(IPluginManager pluginManager, ILogger<PluginsController> logger)
    {
        _pluginManager = pluginManager;
        _logger = logger;
    }

    /// <summary>
    /// Gets all registered plugins.
    /// </summary>
    /// <returns>List of all plugins with their metadata.</returns>
    [HttpGet]
    public IActionResult GetAllPlugins()
    {
        try
        {
            var plugins = _pluginManager.Plugins.Select(p => new PluginDto
            {
                Id = p.Metadata.Id,
                Name = p.Metadata.Name,
                Description = p.Metadata.Description,
                Version = p.Metadata.Version,
                Author = p.Metadata.Author,
                Dependencies = p.Metadata.Dependencies.ToList(),
                IsEnabledByDefault = p.Metadata.IsEnabledByDefault,
                IsRunning = p.IsRunning
            }).ToList();

            return Ok(plugins);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving plugins");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets all running plugins.
    /// </summary>
    /// <returns>List of running plugins.</returns>
    [HttpGet("running")]
    public IActionResult GetRunningPlugins()
    {
        try
        {
            var runningPlugins = _pluginManager.RunningPlugins.Select(p => new PluginDto
            {
                Id = p.Metadata.Id,
                Name = p.Metadata.Name,
                Description = p.Metadata.Description,
                Version = p.Metadata.Version,
                Author = p.Metadata.Author,
                Dependencies = p.Metadata.Dependencies.ToList(),
                IsEnabledByDefault = p.Metadata.IsEnabledByDefault,
                IsRunning = p.IsRunning
            }).ToList();

            return Ok(runningPlugins);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving running plugins");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets a specific plugin by ID.
    /// </summary>
    /// <param name="id">The plugin ID.</param>
    /// <returns>The plugin information.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlugin(string id)
    {
        try
        {
            var plugin = await _pluginManager.GetPluginAsync(id);
            if (plugin == null)
                return NotFound($"Plugin with ID '{id}' not found");

            var pluginDto = new PluginDto
            {
                Id = plugin.Metadata.Id,
                Name = plugin.Metadata.Name,
                Description = plugin.Metadata.Description,
                Version = plugin.Metadata.Version,
                Author = plugin.Metadata.Author,
                Dependencies = plugin.Metadata.Dependencies.ToList(),
                IsEnabledByDefault = plugin.Metadata.IsEnabledByDefault,
                IsRunning = plugin.IsRunning
            };

            return Ok(pluginDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving plugin {PluginId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Starts a plugin by ID.
    /// </summary>
    /// <param name="id">The plugin ID.</param>
    /// <returns>Success or error response.</returns>
    [HttpPost("{id}/start")]
    public async Task<IActionResult> StartPlugin(string id)
    {
        try
        {
            await _pluginManager.StartPluginAsync(id);
            _logger.LogInformation("Plugin {PluginId} started successfully", id);
            return Ok(new { message = $"Plugin '{id}' started successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting plugin {PluginId}", id);
            return StatusCode(500, new { error = "Failed to start plugin", details = ex.Message });
        }
    }

    /// <summary>
    /// Stops a plugin by ID.
    /// </summary>
    /// <param name="id">The plugin ID.</param>
    /// <returns>Success or error response.</returns>
    [HttpPost("{id}/stop")]
    public async Task<IActionResult> StopPlugin(string id)
    {
        try
        {
            await _pluginManager.StopPluginAsync(id);
            _logger.LogInformation("Plugin {PluginId} stopped successfully", id);
            return Ok(new { message = $"Plugin '{id}' stopped successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping plugin {PluginId}", id);
            return StatusCode(500, new { error = "Failed to stop plugin", details = ex.Message });
        }
    }

    /// <summary>
    /// Starts all plugins.
    /// </summary>
    /// <returns>Success or error response.</returns>
    [HttpPost("start-all")]
    public async Task<IActionResult> StartAllPlugins()
    {
        try
        {
            await _pluginManager.StartAllPluginsAsync();
            _logger.LogInformation("All plugins started successfully");
            return Ok(new { message = "All plugins started successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting all plugins");
            return StatusCode(500, new { error = "Failed to start all plugins", details = ex.Message });
        }
    }

    /// <summary>
    /// Stops all plugins.
    /// </summary>
    /// <returns>Success or error response.</returns>
    [HttpPost("stop-all")]
    public async Task<IActionResult> StopAllPlugins()
    {
        try
        {
            await _pluginManager.StopAllPluginsAsync();
            _logger.LogInformation("All plugins stopped successfully");
            return Ok(new { message = "All plugins stopped successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping all plugins");
            return StatusCode(500, new { error = "Failed to stop all plugins", details = ex.Message });
        }
    }
}

/// <summary>
/// Data transfer object for plugin information.
/// </summary>
public class PluginDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = new();
    public bool IsEnabledByDefault { get; set; }
    public bool IsRunning { get; set; }
}