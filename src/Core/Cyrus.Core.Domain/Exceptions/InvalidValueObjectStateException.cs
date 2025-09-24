namespace Cyrus.Core.Domain.Exceptions;

/// <summary>
/// Exception for invalid state or validation failures within domain value objects.
/// </summary>
public sealed class InvalidValueObjectStateException : DomainStateException
{
    /// <summary>
    /// Initializes a new instance of the exception with a message (optionally serving as a format string) and parameters.
    /// </summary>
    /// <param name="message">The error message or composite format string.</param>
    /// <param name="parameters">Format parameters that will be substituted into the message if present.</param>
    public InvalidValueObjectStateException(string message, params string[] parameters) : base(message, parameters)
    {
    }
}
