using Cyrus.Core.RequestResponse.Queries;

namespace Cyrus.Core.Contracts.ApplicationServices.Queries;

/// <summary>
/// Mediator-style dispatcher for queries. Routes a query to its appropriate handler.
/// </summary>
public interface IQueryDispatcher
{
    Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query) where TQuery : class, IQuery<TData>;
    Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query, CancellationToken cancellationToken) where TQuery : class, IQuery<TData>;
}
