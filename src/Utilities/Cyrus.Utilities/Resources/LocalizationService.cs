using System.Globalization;

namespace Cyrus.Utilities.Resources;

/// <summary>
/// Implements culture-aware string lookup with fallback to a default culture.
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private readonly ILocalizationProvider _provider;
    private CultureInfo _defaultCulture;

    public LocalizationService(ILocalizationProvider provider, LocalizationOptions options)
    {
        _provider = provider;
        _defaultCulture = options.DefaultCulture;
    }

    public string this[string key] => GetString(key);

    public string GetString(string key, CultureInfo culture)
        => _provider.GetString(key, culture)
           ?? _provider.GetString(key, _defaultCulture)
           ?? key;

    public string GetString(string key)
        => GetString(key, CultureInfo.CurrentUICulture);

    public void SetDefaultCulture(CultureInfo culture) => _defaultCulture = culture;

    /// <summary>
    /// Creates a default localization service that reads from the conventional path in the app base directory.
    /// </summary>
    public static LocalizationService CreateDefault(LocalizationOptions? options = null)
    {
        options ??= new LocalizationOptions();
        var basePath = Path.Combine(AppContext.BaseDirectory, options.BasePath);
        var provider = new Cyrus.Utilities.Resources.Json.JsonLocalizationProvider(basePath);
        return new LocalizationService(provider, options);
    }
}

