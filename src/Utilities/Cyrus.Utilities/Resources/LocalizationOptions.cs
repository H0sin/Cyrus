using System.Globalization;

namespace Cyrus.Utilities.Resources;

/// <summary>
/// Options for the JSON-based localization provider.
/// </summary>
public sealed class LocalizationOptions
{
    /// <summary>Relative base path to localization JSON files (copied to output).</summary>
    public string BasePath { get; init; } = "Resources/Translators";

    /// <summary>Default culture used for fallback.</summary>
    public CultureInfo DefaultCulture { get; set; } = CultureInfo.GetCultureInfo("en");
}

