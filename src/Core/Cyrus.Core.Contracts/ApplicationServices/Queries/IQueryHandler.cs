using Cyrus.Core.RequestResponse.Queries;

namespace Cyrus.Core.Contracts.ApplicationServices.Queries;

/// <summary>
/// Handles a query and returns a structured result.
/// </summary>
/// <typeparam name="TQuery">Query type that carries input parameters.</typeparam>
/// <typeparam name="TData">Type of the data returned by the query.</typeparam>
public interface IQueryHandler<TQuery, TData>
    where TQuery : class, IQuery<TData>
{
    Task<QueryResult<TData>> Handle(TQuery request);
    Task<QueryResult<TData>> Handle(TQuery request, CancellationToken cancellationToken);
}
