using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cyrus.Core.Contracts.ApplicationServices.Events;
using Cyrus.Core.Domain.Events;
using Cyrus.Utilities.Logging;

namespace Cyrus.Core.ApplicationService.Events;

/// <summary>
/// Resolves and invokes domain event handlers for a given event type.
/// Logs routing start and total handler execution time.
/// </summary>
public sealed class EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
    : IEventDispatcher
{
    private readonly Stopwatch _stopwatch = new();

    /// <inheritdoc />
    public async Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : class, IDomainEvent
        => await PublishDomainEventAsync(@event, CancellationToken.None);

    /// <inheritdoc />
    public async Task PublishDomainEventAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken) where TDomainEvent : class, IDomainEvent
    {
        _stopwatch.Restart();
        var count = 0;
        try
        {
            logger.LogDebug("Routing event of type {EventType} with value {Event} started at {StartDateTime}", @event.GetType(), @event, DateTime.Now);

            var handlers = serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>().ToList();
            count = handlers.Count;

            var tasks = new List<Task>(count);
            foreach (var handler in handlers)
            {
                tasks.Add(handler.Handle(@event, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "No suitable handler for {EventType}. Routing failed at {StartDateTime}.", @event.GetType(), DateTime.Now);
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            logger.LogInformation(CyrusEventId.PerformanceMeasurement, "Handled {Count} handler(s) for {EventType} in {ElapsedMs} ms", count, @event.GetType(), _stopwatch.ElapsedMilliseconds);
        }
    }
}