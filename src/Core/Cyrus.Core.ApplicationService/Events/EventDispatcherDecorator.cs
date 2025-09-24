using Cyrus.Core.Contracts.ApplicationServices.Events;
using Cyrus.Core.Domain.Events;

namespace Cyrus.Core.ApplicationService.Events;

/// <summary>
/// Base type for event dispatcher decorators. Holds a reference to the next dispatcher
/// in the chain and exposes abstract publish methods to be implemented by subclasses.
/// </summary>
public abstract class EventDispatcherDecorator(IEventDispatcher inner) : IEventDispatcher
{
    protected IEventDispatcher Inner { get; private set; } = inner;
    public abstract int Order { get; }

    /// <summary>Replaces the inner dispatcher at runtime (for chaining).</summary>
    public void SetEventDispatcher(IEventDispatcher eventDispatcher) => Inner = eventDispatcher;

    /// <summary>
    /// Publishes a domain event asynchronously.
    /// </summary>
    public abstract Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : class, IDomainEvent;
    
    /// <summary>
    /// Publishes a domain event asynchronously with a cancellation token.
    /// </summary>
    public abstract Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken) where TDomainEvent : class, IDomainEvent;

    Task IEventDispatcher.PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event)
    {
        return PublishDomainEventAsync(@event);
    }
}