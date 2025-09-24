using System.Globalization;

namespace Cyrus.Utilities.Resources;

/// <summary>
/// Abstraction for retrieving localized strings from an underlying store.
/// </summary>
public interface ILocalizationProvider
{
    /// <summary>Gets a localized string for the given key and culture, or null if not found.</summary>
    string? GetString(string key, CultureInfo culture);

    /// <summary>Gets all available cultures supported by the provider.</summary>
    IReadOnlyCollection<CultureInfo> GetSupportedCultures();
}

