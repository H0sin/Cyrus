namespace Cyrus.Core.Domain.Exceptions;

/// <summary>
/// Exception for invalid state transitions or invariant violations within domain entities.
/// </summary>
public sealed class InvalidEntityStateException : DomainStateException
{
    /// <summary>
    /// Initializes a new instance of the exception with a message (optionally serving as a format string) and parameters.
    /// </summary>
    /// <param name="message">The error message or composite format string.</param>
    /// <param name="parameters">Format parameters that will be substituted into the message if present.</param>
    public InvalidEntityStateException(string message, params string[] parameters) : base(message, parameters)
    {
    }
}
