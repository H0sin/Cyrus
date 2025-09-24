namespace Cyrus.Core.RequestResponse.Common;

/// <summary>
/// Standard status codes for application service operations.
/// </summary>
public enum ApplicationServiceStatus
{
    /// <summary>Operation completed successfully.</summary>
    Ok = 1,
    /// <summary>Requested resource was not found.</summary>
    NotFound = 2,
    /// <summary>Validation errors occurred.</summary>
    ValidationError = 3,
    /// <summary>Domain layer reported an invalid state (invariants violated).</summary>
    InvalidDomainState = 4,
    /// <summary>An unhandled exception occurred.</summary>
    Exception = 5,
    /// <summary>Authentication failed or user is not authorized.</summary>
    Unauthorized = 6,
    /// <summary>Operation conflicts with the current state (e.g., duplicate).</summary>
    Conflict = 7
}
