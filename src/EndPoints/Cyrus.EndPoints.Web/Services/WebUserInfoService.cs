using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Cyrus.Utilities.Abstractions;

namespace Cyrus.EndPoints.Web.Services;

/// <summary>
/// Web-aware user info provider that reads user metadata from HttpContext and claims.
/// </summary>
public sealed class WebUserInfoService(
    IHttpContextAccessor httpContextAccessor,
    IOptions<UserManagementOptions> options)
    : IUserInfoService
{
    private readonly UserManagementOptions _options = options.Value;
    
    public string GetUserAgent()
        => httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? _options.DefaultUserAgent;

    public string GetUserIp()
        => httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? _options.DefaultUserIp;

    public string UserId()
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public string GetUsername()
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? _options.DefaultUsername;

    public string GetFirstName()
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName) ?? _options.DefaultFirstName;

    public string GetLastName()
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Surname) ?? _options.DefaultLastName;

    public bool IsCurrentUser(string userId)
        => string.Equals(UserId(), userId, StringComparison.OrdinalIgnoreCase);

    public string? GetClaim(string claimType)
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);

    public string UserIdOrDefault() => UserIdOrDefault(_options.DefaultUserId);

    public string UserIdOrDefault(string defaultValue)
    {
        var id = UserId();
        return string.IsNullOrEmpty(id) ? defaultValue : id;
    }
}
