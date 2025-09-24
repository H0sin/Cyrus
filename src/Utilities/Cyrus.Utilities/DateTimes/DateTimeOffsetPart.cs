namespace Cyrus.Utilities.DateTimes;
/// <summary>
/// Specifies which part of a DateTimeOffset value to return or convert.
/// </summary>
public enum DateTimeOffsetPart
{
    /// <summary>
    /// Returns the DateTime component without applying the offset or converting to the server's local time.
    /// </summary>
    DateTime,

    /// <summary>
    /// Returns the local date and time for the server where the application is running.
    /// </summary>
    LocalDateTime,

    /// <summary>
    /// Returns the Coordinated Universal Time (UTC) date and time of the current DateTimeOffset.
    /// </summary>
    UtcDateTime,

    /// <summary>
    /// Returns the date and time converted to Iran's local time zone.
    /// </summary>
    IranLocalDateTime
}
