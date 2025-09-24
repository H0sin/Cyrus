using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Cyrus.Utilities.Abstractions;
using Cyrus.EndPoints.Web.Services;

namespace Cyrus.EndPoints.Web.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the web-aware IUserInfoService and binds UserManagementOptions from configuration section "UserManagement".
    /// </summary>
    public static IServiceCollection AddWebUserInfoService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<UserManagementOptions>(configuration.GetSection("UserManagement"));
        services.AddHttpContextAccessor();
        services.AddScoped<IUserInfoService, WebUserInfoService>();
        return services;
    }
}