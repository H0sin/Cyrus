namespace Cyrus.Core.RequestResponse.Common;

/// <summary>
/// Fluent extension helpers for <see cref="ApplicationServiceResult"/>.
/// </summary>
public static class ApplicationServiceResultExtensions
{
    /// <summary>
    /// Appends messages to the result and returns the same instance for fluent composition.
    /// </summary>
    public static TSelf WithMessages<TSelf>(this TSelf result, params string[] messages)
        where TSelf : ApplicationServiceResult
    {
        if (messages is { Length: > 0 })
        {
            result.AddMessages(messages);
        }
        return result;
    }
}
