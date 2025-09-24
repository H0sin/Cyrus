using System.Globalization;

namespace Cyrus.Utilities.Resources;

/// <summary>
/// Basic translator implementation over ILocalizationService.
/// </summary>
public sealed class Translator(ILocalizationService localization) : ITranslator
{
    public string this[string key] => localization.GetString(key);
    public string this[string key, params string[] parameters]
    {
        get
        {
            var template = localization.GetString(key);
            return parameters is { Length: > 0 } ? string.Format(CultureInfo.CurrentUICulture, template, parameters) : template;
        }
    }
}

