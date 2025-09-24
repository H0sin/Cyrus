using System.Globalization;

namespace Cyrus.Utilities.Resources;

/// <summary>
/// Service that resolves localized strings with culture fallback.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets a localized string for the current UI culture.
    /// </summary>
    string this[string key] { get; }

    /// <summary>Gets a localized string for the specified culture.</summary>
    string GetString(string key, CultureInfo culture);

    /// <summary>Gets a localized string using the current UI culture with fallback to the default culture.</summary>
    string GetString(string key);

    /// <summary>Changes the default fallback culture.</summary>
    void SetDefaultCulture(CultureInfo culture);
}

