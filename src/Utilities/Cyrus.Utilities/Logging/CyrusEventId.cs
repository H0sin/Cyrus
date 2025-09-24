namespace Cyrus.Utilities.Logging;

/// <summary>
/// Common logging EventId constants used across the application for structured logging.
/// Preferred name: CyrusEventId.
/// </summary>
public static class CyrusEventId
{
    public static readonly int PerformanceMeasurement = 1001;
    public static readonly int DomainValidationException = 1010;
    public static readonly int CommandValidation = 1011;
    public static readonly int QueryValidation = 1012;
    public static readonly int EventValidation = 1013;
}