using Microsoft.Extensions.DependencyInjection;
using Cyrus.Utilities.Resources;
using Cyrus.Utilities.Resources.Json;
using Cyrus.Utilities.Abstractions;
using Cyrus.Utilities.Users;
using Cyrus.Utilities.Excel;

namespace Cyrus.Utilities.Extensions;

/// <summary>
/// DI helpers to wire-up Cyrus.Utilities services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds localization services using the JSON provider. Copies Translator JSON files must be available in output.
    /// </summary>
    public static IServiceCollection AddCyrusUtilities(this IServiceCollection services, Action<LocalizationOptions>? configure = null)
    {
        var options = new LocalizationOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<ILocalizationProvider>(_ =>
        {
            var basePath = Path.Combine(AppContext.BaseDirectory, options.BasePath);
            return new JsonLocalizationProvider(basePath);
        });
        services.AddSingleton<ILocalizationService>(sp =>
            new LocalizationService(sp.GetRequiredService<ILocalizationProvider>(), options));
        services.AddSingleton<ITranslator>(sp => new Translator(sp.GetRequiredService<ILocalizationService>()));

        // Excel
        services.AddSingleton<IExcelSerializer, ClosedXmlExcelSerializer>();

        return services;
    }

    /// <summary>
    /// Registers a development/test user info service implementation.
    /// </summary>
    public static IServiceCollection AddFakeUserInfoService(this IServiceCollection services, string defaultUserId = "1")
    {
        services.AddSingleton<IUserInfoService>(_ => new FakeUserInfoService(defaultUserId));
        return services;
    }
}
