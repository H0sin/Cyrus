namespace Cyrus.Core.Domain.Exceptions;

/// <summary>
/// Base exception for domain-layer validation/state errors originating from entities and value objects.
/// Higher layers can use this to uniformly handle domain validation errors.
/// </summary>
public abstract class DomainStateException : Exception
{
    /// <summary>
    /// Optional message parameters. If present, the base message is treated as a format string.
    /// </summary>
    public string[] Parameters { get; }

    protected DomainStateException(string message, params string[] parameters) : base(message)
    {
        Parameters = parameters ?? Array.Empty<string>();
    }

    public override string ToString()
    {
        if (Parameters.Length == 0)
        {
            return Message;
        }

        var result = Message;
        for (int i = 0; i < Parameters.Length; i++)
        {
            var placeHolder = $"{{{i}}}";
            result = result.Replace(placeHolder, Parameters[i]);
        }
        return result;
    }
}
