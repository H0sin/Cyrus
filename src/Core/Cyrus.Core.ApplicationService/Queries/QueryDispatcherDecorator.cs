using Cyrus.Core.Contracts.ApplicationServices.Queries;
using Cyrus.Core.RequestResponse.Queries;

namespace Cyrus.Core.ApplicationService.Queries;

/// <summary>
/// Base type for query dispatcher decorators. Holds a reference to the next dispatcher in the chain
/// and exposes abstract execute methods to be implemented by subclasses.
/// </summary>
public abstract class QueryDispatcherDecorator(IQueryDispatcher inner) : IQueryDispatcher
{
    protected IQueryDispatcher Inner { get; private set; } = inner;
    public abstract int Order { get; }

    /// <summary>Replaces the inner dispatcher at runtime (for chaining).</summary>
    public void SetQueryDispatcher(IQueryDispatcher queryDispatcher) => Inner = queryDispatcher;

    public abstract Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query) where TQuery : class, IQuery<TData>;
    public abstract Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query, CancellationToken cancellationToken) where TQuery : class, IQuery<TData>;
}