using Cyrus.Core.Contracts.ApplicationServices.Events;
using Microsoft.Extensions.Logging;
using Cyrus.Core.Domain.Exceptions;
using Cyrus.Utilities.Logging;

namespace Cyrus.Core.ApplicationService.Events;

/// <summary>
/// Catches domain exceptions during event publishing and logs them with CyrusEventId.
/// Non-domain exceptions are rethrown to be handled upstream.
/// </summary>
public class EventDispatcherDomainExceptionHandlerDecorator(
    ILogger<EventDispatcherDomainExceptionHandlerDecorator> logger,
    IEventDispatcher inner)
    : EventDispatcherDecorator(inner)
{
    public override int Order => 2;

    public override Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event)
        => PublishDomainEventAsync(@event, CancellationToken.None);

    public override async Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            await Inner.PublishDomainEventAsync(@event, cancellationToken);
        }
        catch (DomainStateException ex)
        {
            logger.LogError(CyrusEventId.DomainValidationException, ex, "Processing of {EventType} With value {Event} failed at {StartDateTime} because there are domain exceptions.", @event.GetType(), @event, DateTime.Now);
        }
        catch (AggregateException ex) when (ex.InnerException is DomainStateException domainStateException)
        {
            logger.LogError(CyrusEventId.DomainValidationException, ex, "Processing of {EventType} With value {Event} failed at {StartDateTime} because there are domain exceptions.", @event.GetType(), @event, DateTime.Now);
        }
        catch
        {
            throw;
        }
    }
}