using Cyrus.Core.Domain.Events;

namespace Cyrus.Core.Contracts.Data.Commands;

/// <summary>
/// Contract for persisting and retrieving domain events for aggregates (event store).
/// Use this to append new events and to load an aggregate's event stream for rehydration.
/// </summary>
public interface IDomainEventStore
{
    /// <summary>
    /// Appends a batch of domain events to the aggregate's stream.
    /// </summary>
    /// <typeparam name="TEvent">Type of domain event.</typeparam>
    /// <param name="aggregateName">Aggregate type/name (stream category).</param>
    /// <param name="aggregateId">Aggregate identifier (stream id).</param>
    /// <param name="events">Events to append in order.</param>
    void Save<TEvent>(string aggregateName, string aggregateId, IEnumerable<TEvent> events) where TEvent : IDomainEvent;

    /// <summary>
    /// Appends a batch of domain events to the aggregate's stream asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">Type of domain event.</typeparam>
    /// <param name="aggregateName">Aggregate type/name (stream category).</param>
    /// <param name="aggregateId">Aggregate identifier (stream id).</param>
    /// <param name="events">Events to append in order.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    Task SaveAsync<TEvent>(string aggregateName, string aggregateId, IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;

    /// <summary>
    /// Loads domain events for the given aggregate from the specified starting version.
    /// </summary>
    /// <param name="aggregateName">Aggregate type/name (stream category).</param>
    /// <param name="aggregateId">Aggregate identifier (stream id).</param>
    /// <param name="fromVersion">Optional starting version (inclusive). Defaults to 0.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <returns>Ordered list of domain events.</returns>
    Task<IReadOnlyList<IDomainEvent>> LoadAsync(string aggregateName, string aggregateId, long fromVersion = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the current (last) version of the aggregate's event stream.
    /// </summary>
    /// <param name="aggregateName">Aggregate type/name (stream category).</param>
    /// <param name="aggregateId">Aggregate identifier (stream id).</param>
    long? GetCurrentVersion(string aggregateName, string aggregateId);

    /// <summary>
    /// Returns the current (last) version of the aggregate's event stream asynchronously.
    /// </summary>
    /// <param name="aggregateName">Aggregate type/name (stream category).</param>
    /// <param name="aggregateId">Aggregate identifier (stream id).</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    Task<long?> GetCurrentVersionAsync(string aggregateName, string aggregateId, CancellationToken cancellationToken = default);
}
