using Microsoft.EntityFrameworkCore;
using Cyrus.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Cyrus.Utilities.Abstractions;

namespace Cyrus.Infrastructure.Data.Sql.Commands.Extensions;
public static class AuditableShadowProperties
{
    public static readonly Func<object, string> EfPropertyCreatedByUserId =
        entity => EF.Property<string>(entity, CreatedByUserId);
    public const string CreatedByUserId = nameof(CreatedByUserId);

    public static readonly Func<object, string> EfPropertyModifiedByUserId =
        entity => EF.Property<string>(entity, ModifiedByUserId);
    public const string ModifiedByUserId = nameof(ModifiedByUserId);

    public static readonly Func<object, DateTime?> EfPropertyCreatedDateTime =
        entity => EF.Property<DateTime?>(entity, CreatedDateTime);
    public const string CreatedDateTime = nameof(CreatedDateTime);

    public static readonly Func<object, DateTime?> EfPropertyModifiedDateTime =
        entity => EF.Property<DateTime?>(entity, ModifiedDateTime);
    public const string ModifiedDateTime = nameof(ModifiedDateTime);

    public static void AddAuditableShadowProperties(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(c => typeof(IAuditableEntity).IsAssignableFrom(c.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property<string>(CreatedByUserId).HasMaxLength(50);
            modelBuilder.Entity(entityType.ClrType)
                .Property<string>(ModifiedByUserId).HasMaxLength(50);
            modelBuilder.Entity(entityType.ClrType)
                .Property<DateTime?>(CreatedDateTime);
            modelBuilder.Entity(entityType.ClrType)
                .Property<DateTime?>(ModifiedDateTime);
        }
    }

    public static void SetAuditableEntityPropertyValues(this ChangeTracker changeTracker, IUserInfoService userInfoService)
    {
        var now = DateTime.UtcNow;
        var userId = userInfoService.UserIdOrDefault();

        foreach (var entry in changeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(CreatedDateTime).CurrentValue = now;
                entry.Property(CreatedByUserId).CurrentValue = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(ModifiedDateTime).CurrentValue = now;
                entry.Property(ModifiedByUserId).CurrentValue = userId;
            }
        }
    }
}
