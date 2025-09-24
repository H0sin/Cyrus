using System.Linq;
using Cyrus.Core.RequestResponse.Common;
using Cyrus.Core.RequestResponse.Queries;
using Cyrus.Core.Contracts.ApplicationServices.Queries;
using Cyrus.Utilities.Resources;

namespace Cyrus.Core.ApplicationService.Queries;

/// <summary>
/// Base class for query handlers returning a typed payload. Provides helper methods
/// to build localized results and supports cancellation tokens.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TData">The result payload type.</typeparam>
public abstract class QueryHandler<TQuery, TData> : IQueryHandler<TQuery, TData>
    where TQuery : class, IQuery<TData>
{
    protected readonly ITranslator Translator;

    protected QueryHandler(ITranslator translator)
    {
        Translator = translator;
    }

    /// <summary>
    /// Handles the query using the default cancellation token.
    /// </summary>
    public Task<QueryResult<TData>> Handle(TQuery query)
        => Handle(query, CancellationToken.None);

    /// <summary>
    /// Handles the query with a cancellation token. Implement in derived classes.
    /// </summary>
    public abstract Task<QueryResult<TData>> Handle(TQuery query, CancellationToken cancellationToken);

    // Helpers
    protected QueryResult<TData> Ok(TData data, params string[] messageKeys)
    {
        var res = QueryResult<TData>.Ok(data);
        if (messageKeys is { Length: > 0 }) res.AddMessages(TranslateKeys(messageKeys));
        return res;
    }

    protected Task<QueryResult<TData>> OkAsync(TData data, params string[] messageKeys)
        => Task.FromResult(Ok(data, messageKeys));

    protected QueryResult<TData> Result(TData data)
        => Ok(data); // Ok returns whatever data is provided; caller decides NotFound when null if needed

    protected QueryResult<TData> NotFound(params string[] messageKeys)
    {
        var res = new QueryResult<TData> { Status = ApplicationServiceStatus.NotFound };
        if (messageKeys is { Length: > 0 }) res.AddMessages(TranslateKeys(messageKeys));
        return res;
    }

    protected void AddMessage(QueryResult<TData> result, string key)
        => result.AddMessage(Translator[key]);

    protected void AddMessage(QueryResult<TData> result, string key, params string[] arguments)
        => result.AddMessage(Translator[key, arguments]);

    protected string T(string key) => Translator[key];
    protected string T(string key, params string[] args) => Translator[key, args];

    private IEnumerable<string> TranslateKeys(IEnumerable<string> keys)
        => keys.Select(k => Translator[k]);
}
