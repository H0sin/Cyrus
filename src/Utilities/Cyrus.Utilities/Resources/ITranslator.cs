namespace Cyrus.Utilities.Resources;

public interface ITranslator
{
    string this[string key] { get; }
    string this[string key, params string[] parameters] { get; }
}

