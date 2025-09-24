using Cyrus.Core.Domain.Events;

namespace Cyrus.Core.Domain.Entities;

/// <summary>
/// Marker interface for DDD Aggregate Roots. Aggregates can raise domain events to be persisted/published
/// by the application layer. Implementations should buffer events and expose them via <see cref="GetEvents"/>
/// and clear them using <see cref="ClearEvents"/> after successful commit.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Removes all buffered domain events after they have been persisted/published.
    /// </summary>
    void ClearEvents();

    /// <summary>
    /// Returns the sequence of domain events raised by the aggregate during the current unit of work.
    /// Consumers should treat the returned sequence as read-only.
    /// </summary>
    IEnumerable<IDomainEvent> GetEvents();
}