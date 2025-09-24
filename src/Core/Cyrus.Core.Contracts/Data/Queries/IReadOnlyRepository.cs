using System.Linq.Expressions;
using Cyrus.Core.RequestResponse.Queries;
using Cyrus.Core.Domain.ValueObjects;

namespace Cyrus.Core.Contracts.Data.Queries;

/// <summary>
/// Generic read-only repository contract for query-side data access.
/// </summary>
/// <typeparam name="TEntity">Entity DTO or read model type.</typeparam>
/// <typeparam name="TId">Strongly-typed identifier for the entity.</typeparam>
public interface IReadOnlyRepository<TEntity, TId>
{
    /// <summary>Finds an entity by its identifier.</summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>Finds an entity by its business identifier.</summary>
    Task<TEntity?> GetByBusinessIdAsync(BusinessId businessId, CancellationToken cancellationToken = default);

    /// <summary>Determines whether any entities match the predicate.</summary>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>Returns a list of entities matching the predicate, optionally paged and sorted.</summary>
    Task<List<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        int? skip = null,
        int? take = null,
        IEnumerable<SortDescriptor>? sorts = null,
        CancellationToken cancellationToken = default);
}

