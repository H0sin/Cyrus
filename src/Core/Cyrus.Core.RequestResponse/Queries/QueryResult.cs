using Cyrus.Core.RequestResponse.Common;

namespace Cyrus.Core.RequestResponse.Queries;

/// <summary>
/// Encapsulates the result of executing a query, including status, messages and an optional data payload.
/// </summary>
/// <typeparam name="TData">Type of the data payload.</typeparam>
public sealed class QueryResult<TData> : ApplicationServiceResult
{
    /// <summary>Optional data payload returned by the query.</summary>
    public TData? Data { get; init; }

    /// <summary>Indicates whether any non-default data payload is present.</summary>
    public bool HasData => !EqualityComparer<TData?>.Default.Equals(Data, default);

    /// <summary>Creates a successful result with the provided data.</summary>
    public static QueryResult<TData> Ok(TData? data)
        => new QueryResult<TData> { Status = ApplicationServiceStatus.Ok, Data = data };

    /// <summary>Creates a NotFound result.</summary>
    public static QueryResult<TData> NotFound(params string[] messages)
    {
        var r = new QueryResult<TData> { Status = ApplicationServiceStatus.NotFound };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }

    /// <summary>Creates a validation error result.
    /// Messages can contain user-facing validation messages.</summary>
    public static QueryResult<TData> ValidationError(params string[] messages)
    {
        var r = new QueryResult<TData> { Status = ApplicationServiceStatus.ValidationError };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }

    /// <summary>Creates a generic error result.</summary>
    public static QueryResult<TData> Error(params string[] messages)
    {
        var r = new QueryResult<TData> { Status = ApplicationServiceStatus.Exception };
        if (messages is { Length: > 0 }) r.AddMessages(messages);
        return r;
    }
}
