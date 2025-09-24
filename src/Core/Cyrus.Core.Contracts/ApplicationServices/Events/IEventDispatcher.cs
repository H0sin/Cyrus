using Cyrus.Core.Domain.Events;

namespace Cyrus.Core.Contracts.ApplicationServices.Events;

/// <summary>
/// Publishes domain events to their respective handlers.
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Publishes a domain event asynchronously.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="event">The domain event to publish.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : class, IDomainEvent;

    /// <summary>
    /// Publishes a domain event asynchronously with a cancellation token.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="event">The domain event to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken) where TDomainEvent : class, IDomainEvent;
}