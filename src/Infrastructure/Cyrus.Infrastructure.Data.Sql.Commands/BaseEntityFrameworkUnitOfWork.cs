using Cyrus.Core.Contracts.Data.Commands;

namespace Cyrus.Infrastructure.Data.Sql.Commands;

public abstract class BaseEntityFrameworkUnitOfWork<TDbContext>(TDbContext dbContext) : IUnitOfWork
    where TDbContext : BaseCommandDbContext
{
    protected readonly TDbContext DbContext = dbContext;

    public void BeginTransaction()
    {
        DbContext.BeginTransaction();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await DbContext.BeginTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await DbContext.RollbackTransactionAsync(cancellationToken);
    }

    public int Commit()
    {
        var result = DbContext.SaveChanges();
        return result;
    }

    public async Task<int> CommitAsync()
    {
        var result = await DbContext.SaveChangesAsync();
        return result;
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        var result = DbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    public void CommitTransaction()
    {
        DbContext.CommitTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
         await DbContext.CommitTransactionAsync(cancellationToken);
    }

    public void RollbackTransaction()
    {
        DbContext.RollbackTransaction();
    }
}