namespace Cyrus.Core.Contracts.Data.Commands;

/// <summary>
/// Unit of Work contract to coordinate transactions and persistence across repositories.
/// See: https://martinfowler.com/eaaCatalog/unitOfWork.html
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Begins a database transaction if manual transaction control is required.</summary>
    void BeginTransaction();

    /// <summary>Begins a database transaction asynchronously.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits the current transaction when using manual transaction control.</summary>
    void CommitTransaction();

    /// <summary>Commits the current transaction asynchronously when using manual transaction control.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back the current transaction.</summary>
    void RollbackTransaction();

    /// <summary>Rolls back the current transaction asynchronously.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits pending changes when using implicit transactions.</summary>
    int Commit();

    /// <summary>Commits pending changes asynchronously when using implicit transactions.</summary>
    Task<int> CommitAsync();

    /// <summary>Commits pending changes asynchronously when using implicit transactions with cancellation support.</summary>
    Task<int> CommitAsync(CancellationToken cancellationToken);
}
