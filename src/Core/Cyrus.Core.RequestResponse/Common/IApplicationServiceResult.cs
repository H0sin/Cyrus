namespace Cyrus.Core.RequestResponse.Common;

/// <summary>
/// Contract for results returned by application services, including a status code and optional messages.
/// </summary>
public interface IApplicationServiceResult
{
    /// <summary>Messages associated with the operation result.</summary>
    IEnumerable<string> Messages { get; }

    /// <summary>High-level status of the operation.</summary>
    ApplicationServiceStatus Status { get; set; }
}