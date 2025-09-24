using System.Linq.Expressions;
using Cyrus.Core.Domain.Entities;
using Cyrus.Core.Domain.ValueObjects;

namespace Cyrus.Core.Contracts.Data.Commands;

/// <summary>
/// Base contract for command-side repositories handling aggregate persistence (write model).
/// </summary>
/// <typeparam name="TEntity">Aggregate root type.</typeparam>
/// <typeparam name="TId">Strongly-typed identifier of the aggregate.</typeparam>
public interface ICommandRepository<TEntity, TId> : IUnitOfWork
    where TEntity : AggregateRoot<TId>
    where TId : struct,
          IComparable,
          IComparable<TId>,
          IConvertible,
          IEquatable<TId>,
          IFormattable
{
    /// <summary>Deletes an entity by id.</summary>
    void Delete(TId id);

    /// <summary>Deletes an entity and its related graph (children) by id.</summary>
    void DeleteGraph(TId id);

    /// <summary>Deletes the provided entity instance.</summary>
    void Delete(TEntity entity);

    /// <summary>Inserts a new entity instance.</summary>
    void Insert(TEntity entity);

    /// <summary>Asynchronously inserts a new entity instance.</summary>
    Task InsertAsync(TEntity entity);

    /// <summary>Asynchronously inserts a new entity instance with cancellation support.</summary>
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>Gets an entity by id.</summary>
    TEntity Get(TId id);

    /// <summary>Gets an entity by id asynchronously.</summary>
    Task<TEntity> GetAsync(TId id);

    /// <summary>Gets an entity by id asynchronously with cancellation support.</summary>
    Task<TEntity> GetAsync(TId id, CancellationToken cancellationToken);

    /// <summary>Gets an entity by business id.</summary>
    TEntity? Get(BusinessId businessId);

    /// <summary>Gets an entity by business id asynchronously.</summary>
    Task<TEntity> GetAsync(BusinessId businessId);

    /// <summary>Gets an entity by business id asynchronously with cancellation support.</summary>
    Task<TEntity> GetAsync(BusinessId businessId, CancellationToken cancellationToken);

    /// <summary>Gets an entity graph (with related children) by id.</summary>
    TEntity GetGraph(TId id);

    /// <summary>Gets an entity graph (with related children) by id asynchronously.</summary>
    Task<TEntity?> GetGraphAsync(TId id);

    /// <summary>Gets an entity graph (with related children) by id asynchronously with cancellation support.</summary>
    Task<TEntity> GetGraphAsync(TId id, CancellationToken cancellationToken);

    /// <summary>Gets an entity graph (with related children) by business id.</summary>
    TEntity GetGraph(BusinessId businessId);

    /// <summary>Gets an entity graph (with related children) by business id asynchronously.</summary>
    Task<TEntity?> GetGraphAsync(BusinessId businessId);

    /// <summary>Gets an entity graph (with related children) by business id asynchronously with cancellation support.</summary>
    Task<TEntity> GetGraphAsync(BusinessId businessId, CancellationToken cancellationToken);

    /// <summary>Determines whether any entities satisfy the specified predicate.</summary>
    bool Exists(Expression<Func<TEntity, bool>> expression);

    /// <summary>Determines whether any entities satisfy the specified predicate asynchronously.</summary>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression);

    /// <summary>Determines whether any entities satisfy the specified predicate asynchronously with cancellation support.</summary>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken);
}
