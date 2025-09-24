using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cyrus.Core.Contracts.Data.Commands;
using Cyrus.Core.Domain.Entities;
using Cyrus.Core.Domain.ValueObjects;

namespace Cyrus.Infrastructure.Data.Sql.Commands;

public class BaseCommandRepository<TEntity, TDbContext, TId>(TDbContext dbContext)
    : ICommandRepository<TEntity, TId>, IUnitOfWork
    where TEntity : AggregateRoot<TId>
    where TDbContext : BaseCommandDbContext
    where TId : struct,
    IComparable,
    IComparable<TId>,
    IConvertible,
    IEquatable<TId>,
    IFormattable
{
    protected readonly TDbContext DbContext = dbContext;

    public void Delete(TId id)
    {
        var entity = DbContext.Set<TEntity>().Find(id);
        DbContext.Set<TEntity>().Remove(entity);
    }

    public void Delete(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    public void DeleteGraph(TId id)
    {
        var entity = GetGraph(id);
        if (entity is not null && !entity.Id.Equals(default))
            DbContext.Set<TEntity>().Remove(entity);
    }

    public void Insert(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public async Task InsertAsync(TEntity entity)
    {
        await DbContext.Set<TEntity>().AddAsync(entity);
    }

    public Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
    {
        return DbContext.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();
    }

    public TEntity Get(TId id)
    {
        return DbContext.Set<TEntity>().Find(id);
    }

    public Task<TEntity> GetAsync(TId id, CancellationToken cancellationToken)
    {
        return DbContext.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).AsTask();
    }

    public TEntity? Get(BusinessId businessId)
    {
        return DbContext.Set<TEntity>().FirstOrDefault(c => c.BusinessId == businessId);
    }

    public async Task<TEntity> GetAsync(TId id)
    {
        return await DbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> GetAsync(BusinessId businessId)
    {
        return await DbContext.Set<TEntity>().FirstOrDefaultAsync(c => c.BusinessId == businessId);
    }

    public Task<TEntity> GetAsync(BusinessId businessId, CancellationToken cancellationToken)
    {
        return DbContext.Set<TEntity>().FirstOrDefaultAsync(c => c.BusinessId == businessId, cancellationToken)!;
    }

    public TEntity GetGraph(TId id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = DbContext.Set<TEntity>().AsQueryable();
        var temp = graphPath.ToList();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }

        return query.FirstOrDefault(c => c.Id.Equals(id));
    }

    public Task<TEntity> GetGraphAsync(TId id, CancellationToken cancellationToken)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }

        return query.FirstOrDefaultAsync(c => c.Id.Equals(id), cancellationToken)!;
    }

    public TEntity GetGraph(BusinessId businessId)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = DbContext.Set<TEntity>().AsQueryable();
        var temp = graphPath.ToList();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }

        return query.FirstOrDefault(c => c.BusinessId == businessId);
    }

    public async Task<TEntity?> GetGraphAsync(TId id)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity?> query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }

        return await query.FirstOrDefaultAsync(c => c.Id.Equals(id));
    }

    public async Task<TEntity?> GetGraphAsync(BusinessId businessId)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity?> query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }

        return await query.FirstOrDefaultAsync(c => c.BusinessId == businessId);
    }

    public Task<TEntity> GetGraphAsync(BusinessId businessId, CancellationToken cancellationToken)
    {
        var graphPath = DbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = DbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }

        return query.FirstOrDefaultAsync(c => c.BusinessId == businessId, cancellationToken)!;
    }

    public bool Exists(Expression<Func<TEntity, bool>> expression)
    {
        return DbContext.Set<TEntity>().Any(expression);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await DbContext.Set<TEntity>().AnyAsync(expression);
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken)
    {
        return DbContext.Set<TEntity>().AnyAsync(expression, cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        DbContext.RollbackTransaction();
        return Task.CompletedTask;
    }

    public int Commit()
    {
        return DbContext.SaveChanges();
    }

    public Task<int> CommitAsync()
    {
        return DbContext.SaveChangesAsync();
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        return DbContext.SaveChangesAsync(cancellationToken);
    }

    public void BeginTransaction()
    {
        DbContext.BeginTransaction();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Delegate to sync implementation to keep transaction lifecycle centralized in the DbContext
        await DbContext.BeginTransactionAsync(cancellationToken);
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

public class BaseCommandRepository<TEntity, TDbContext>(TDbContext dbContext)
    : BaseCommandRepository<TEntity, TDbContext, long>(dbContext) where TEntity : AggregateRoot
    where TDbContext : BaseCommandDbContext;