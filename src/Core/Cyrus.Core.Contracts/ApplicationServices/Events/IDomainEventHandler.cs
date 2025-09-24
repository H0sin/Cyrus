using Cyrus.Core.Domain.Events;

namespace Cyrus.Core.Contracts.ApplicationServices.Events;

/// <summary>
/// Handles a domain event raised by aggregates.
/// </summary>
public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent @event);
    Task Handle(TDomainEvent @event, CancellationToken cancellationToken);
}