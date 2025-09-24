using Cyrus.Utilities;

namespace Cyrus.EndPoints.Web.Extensions.DependencyInjection;

public static class AddZaminServicesExtensions
{
    public static IServiceCollection AddZaminUtilityServices(
        this IServiceCollection services)
    {
        services.AddTransient<CyrusService>();
        return services;
    }
}