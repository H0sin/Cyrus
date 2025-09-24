using System.Globalization;
using Cyrus.Core.Domain.Toolkits.ValueObjects;
using Cyrus.Core.Domain.ValueObjects;
using Cyrus.Infrastructure.Data.Sql.Commands.Extensions;
using Cyrus.Infrastructure.Data.Sql.Commands.ValueConversions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cyrus.Infrastructure.Data.Sql.Commands;

public abstract class BaseCommandDbContext : DbContext
{
    protected IDbContextTransaction Transaction;

    public BaseCommandDbContext(DbContextOptions options, IDbContextTransaction transaction) : base(options)
    {
        Transaction = transaction;
    }

    protected BaseCommandDbContext(IDbContextTransaction transaction)
    {
        Transaction = transaction;
    }

    public void BeginTransaction()
    {
        Transaction = Database.BeginTransaction();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Transaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction == null)
        {
            throw new NullReferenceException("Please call `BeginTransaction()` method first.");
        }

        await Transaction.RollbackAsync(cancellationToken);
    }


    public void RollbackTransaction()
    {
        if (Transaction == null)
        {
            throw new NullReferenceException("Please call `BeginTransaction()` method first.");
        }

        Transaction.Rollback();
    }

    public void CommitTransaction()
    {
        if (Transaction == null)
        {
            throw new NullReferenceException("Please call `BeginTransaction()` method first.");
        }

        Transaction.Commit();
    }
    
    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if (Transaction == null)
        {
            throw new NullReferenceException("Please call `BeginTransaction()` method first.");
        }

        await Transaction.CommitAsync(cancellationToken);
    }

    public T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
    {
        var value = Entry(entity).Property(propertyName).CurrentValue;
        return value != null
            ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
            : default;
    }

    public object GetShadowPropertyValue(object entity, string propertyName)
    {
        return Entry(entity).Property(propertyName).CurrentValue;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.AddAuditableShadowProperties();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<Description>().HaveConversion<DescriptionConversion>();
        configurationBuilder.Properties<Title>().HaveConversion<TitleConversion>();
        configurationBuilder.Properties<BusinessId>().HaveConversion<BusinessIdConversion>();
        configurationBuilder.Properties<LegalNationalId>().HaveConversion<LegalNationalId>();
        configurationBuilder.Properties<NationalCode>().HaveConversion<NationalCodeConversion>();
    }

    public IEnumerable<string> GetIncludePaths(Type clrEntityType)
    {
        var entityType = Model.FindEntityType(clrEntityType);
        var includedNavigations = new HashSet<INavigation>();
        var stack = new Stack<IEnumerator<INavigation>>();
        while (true)
        {
            var entityNavigations = new List<INavigation>();
            foreach (var navigation in entityType.GetNavigations())
            {
                if (includedNavigations.Add(navigation))
                    entityNavigations.Add(navigation);
            }

            if (entityNavigations.Count == 0)
            {
                if (stack.Count > 0)
                    yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
            }
            else
            {
                foreach (var navigation in entityNavigations)
                {
                    var inverseNavigation = navigation.Inverse;
                    if (inverseNavigation != null)
                        includedNavigations.Add(inverseNavigation);
                }

                stack.Push(entityNavigations.GetEnumerator());
            }

            while (stack.Count > 0 && !stack.Peek().MoveNext())
                stack.Pop();
            if (stack.Count == 0) break;
            entityType = stack.Peek().Current.TargetEntityType;
        }
    }
}