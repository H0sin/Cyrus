using Cyrus.Core.Contracts.ApplicationServices.Events;
using Cyrus.Utilities.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Cyrus.Core.ApplicationService.Events;

/// <summary>
/// Validates domain events using FluentValidation before dispatching to the next dispatcher.
/// Logs validation start, success, and failures.
/// </summary>
public class EventDispatcherValidationDecorator(
    IServiceProvider serviceProvider,
    ILogger<EventDispatcherValidationDecorator> logger,
    IEventDispatcher inner)
    : EventDispatcherDecorator(inner)
{
    public override int Order => 1;

    public override Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event)
        => PublishDomainEventAsync(@event, CancellationToken.None);

    public override async Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogDebug(CyrusEventId.EventValidation, "Validating Event of type {EventType} With value {Event}  start at :{StartDateTime}", @event.GetType(), @event, DateTime.Now);

        var errors = Validate(@event);

        if (errors.Any())
        {
            logger.LogInformation(CyrusEventId.EventValidation, "Validating query of type {QueryType} With value {Query}  failed. Validation errors are: {ValidationErrors}", @event.GetType(), @event, errors);
            return;
        }

        logger.LogDebug(CyrusEventId.EventValidation, "Validating query of type {QueryType} With value {Query}  finished at :{EndDateTime}", @event.GetType(), @event, DateTime.Now);
        await Inner.PublishDomainEventAsync(@event, cancellationToken);
    }

    private List<string> Validate<TDomainEvent>(TDomainEvent @event)
    {
        var validator = serviceProvider.GetService<IValidator<TDomainEvent>>();
        if (validator is null)
        {
            logger.LogInformation(CyrusEventId.CommandValidation, "There is not any validator for {EventType}", @event.GetType());
            return new List<string>();
        }

        var result = validator.Validate(@event);
        return result.IsValid ? new List<string>() : result.Errors.Select(e => e.ErrorMessage).ToList();
    }
}