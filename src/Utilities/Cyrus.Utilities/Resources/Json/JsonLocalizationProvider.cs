using System.Globalization;
using System.Text.Json;

namespace Cyrus.Utilities.Resources.Json;

/// <summary>
/// Reads localization key-value pairs from JSON files per culture (e.g., en.json, fa.json).
/// </summary>
public sealed class JsonLocalizationProvider : ILocalizationProvider
{
    private readonly string _basePath;
    private readonly Dictionary<string, Dictionary<string, string>> _cache = new(StringComparer.OrdinalIgnoreCase);

    public JsonLocalizationProvider(string basePath)
    {
        _basePath = basePath;
        LoadAll();
    }

    private void LoadAll()
    {
        if (!Directory.Exists(_basePath)) return;
        foreach (var file in Directory.EnumerateFiles(_basePath, "*.json", SearchOption.TopDirectoryOnly))
        {
            var cultureName = Path.GetFileNameWithoutExtension(file);
            try
            {
                var json = File.ReadAllText(file);
                var map = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
                _cache[cultureName] = map;
            }
            catch
            {
                // ignore malformed files to keep provider resilient
            }
        }
    }

    public string? GetString(string key, CultureInfo culture)
    {
        // Try specific culture, then neutral name, then no match
        if (_cache.TryGetValue(culture.Name, out var dict) && dict.TryGetValue(key, out var specific))
            return specific;
        if (!string.Equals(culture.Name, culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
        {
            if (_cache.TryGetValue(culture.TwoLetterISOLanguageName, out var neutral) && neutral.TryGetValue(key, out var neutralVal))
                return neutralVal;
        }
        return null;
    }

    public IReadOnlyCollection<CultureInfo> GetSupportedCultures()
        => _cache.Keys.Select(CultureInfo.GetCultureInfo).ToArray();
}

