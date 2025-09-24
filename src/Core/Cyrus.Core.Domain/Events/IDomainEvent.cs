namespace Cyrus.Core.Domain.Events;

/// <summary>
/// Marker interface for domain events. Events capture meaningful business state changes
/// and are typically persisted and/or published so other parts of the system can react.
/// </summary>
public interface IDomainEvent;
