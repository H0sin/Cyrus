using Cyrus.Core.RequestResponse.Common;

namespace Cyrus.Core.RequestResponse.Commands;

/// <summary>
/// Represents the result of executing an application command (create/update/delete, etc.).
/// </summary>
public class CommandResult : ApplicationServiceResult
{
    public static CommandResult Ok(params string[] messages)
    {
        var r = new CommandResult { Status = ApplicationServiceStatus.Ok };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }

    public static CommandResult NotFound(params string[] messages)
    {
        var r = new CommandResult { Status = ApplicationServiceStatus.NotFound };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }

    public static CommandResult ValidationError(params string[] messages)
    {
        var r = new CommandResult { Status = ApplicationServiceStatus.ValidationError };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }

    public static CommandResult Conflict(params string[] messages)
    {
        var r = new CommandResult { Status = ApplicationServiceStatus.Conflict };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }

    public static CommandResult Error(params string[] messages)
    {
        var r = new CommandResult { Status = ApplicationServiceStatus.Exception };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }
}

/// <summary>
/// Generic result for commands that also return a data payload (e.g., newly created ID).
/// </summary>
/// <typeparam name="TData">Payload type for the command result.</typeparam>
public class CommandResult<TData> : CommandResult
{
    public TData? Data { get; init; }

    /// <summary>Indicates whether any non-default data payload is present.</summary>
    public bool HasData => !EqualityComparer<TData?>.Default.Equals(Data, default);

    public static CommandResult<TData> Ok(TData? data, params string[] messages)
    {
        var r = new CommandResult<TData> { Status = ApplicationServiceStatus.Ok, Data = data };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }
}