namespace Cyrus.Core.Domain.Entities;

/// <summary>
/// Marker interface indicating that an entity participates in audit metadata (created/modified info).
/// Actual audit fields are typically implemented in infrastructure via interceptors/EF configurations.
/// </summary>
public interface IAuditableEntity;