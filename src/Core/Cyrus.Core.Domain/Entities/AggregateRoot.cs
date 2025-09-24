using System.Reflection;
using Cyrus.Core.Domain.Events;

namespace Cyrus.Core.Domain.Entities;

/// <summary>
/// Base class for DDD Aggregate Roots with lightweight event-sourcing support.
/// It collects domain events raised by the aggregate and can rebuild state from a stream of events.
/// </summary>
/// <typeparam name="TId">
/// Strongly-typed identifier for the aggregate. Must be a value type implementing the listed interfaces
/// to support comparison, formatting, and equality.
/// </typeparam>
/// <remarks>
/// Usage:
/// - Command methods on your aggregate should validate invariants and then call <see cref="Apply(IDomainEvent)"/>
///   with a domain event describing the state transition.
/// - State changes are applied by convention-based handlers: a non-public instance method named <c>On</c>
///   with a single parameter whose type matches the event. Example: <c>private void On(CustomerRenamed e)</c>.
/// - The raised events are buffered in-memory and can be retrieved via <see cref="GetEvents"/> for persistence
///   and/or publication, then cleared with <see cref="ClearEvents"/> after successful commit.
/// - The rehydration constructor applies an event stream in order to rebuild the aggregate state.
///
/// Notes:
/// - This base class does not implement versioning or concurrency checks; handle them in repositories/unit of work.
/// - Thread-safety is not provided; aggregates are intended to be used within a single logical unit of work.
/// - If an appropriate <c>On</c> handler is missing for an event type, applying that event will result in an exception.
/// 
/// Reference:
/// https://martinfowler.com/bliki/DDD_Aggregate.html
/// </remarks>
/// <example>
/// Example of a simple aggregate using Apply and On handlers:
/// <code>
/// public sealed class Customer : AggregateRoot<Guid>
/// {
///     private string _name;
///
///     public void Rename(string newName)
///     {
///         if (string.IsNullOrWhiteSpace(newName))
///             throw new ArgumentException("Name is required.", nameof(newName));
///
///         // Raise and apply the domain event
///         Apply(new CustomerRenamed(Id, newName));
///     }
///
///     // Event-sourced state transition
///     private void On(CustomerRenamed e)
///     {
///         _name = e.Name;
///     }
/// }
/// </code>
/// </example>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : struct,
          IComparable,
          IComparable<TId>,
          IConvertible,
          IEquatable<TId>,
          IFormattable
{
    /// <summary>
    /// Holds the domain events raised by this aggregate during the current unit of work.
    /// </summary>
    private readonly List<IDomainEvent> _events = new();

    /// <summary>
    /// Initializes a new aggregate root with an empty event buffer.
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Rehydrates the aggregate by applying the provided event stream in order.
    /// </summary>
    /// <param name="events">
    /// Previously persisted domain events for this aggregate. If null or empty, no action is taken.
    /// The events should be ordered (e.g., by version or timestamp).
    /// </param>
    /// <remarks>
    /// Each event is applied via the <c>On(TEvent)</c> handler. If a matching handler is missing,
    /// an <see cref="InvalidOperationException"/> will be thrown during rehydration.
    /// </remarks>
    protected AggregateRoot(IEnumerable<IDomainEvent> events)
    {
        if (events == null) return;
        foreach (var @event in events)
        {
            Mutate(@event);
        }
    }

    /// <summary>
    /// Applies a domain event to the aggregate state and appends it to the pending event buffer.
    /// </summary>
    /// <param name="@event">The domain event to apply and record.</param>
    /// <remarks>
    /// Preconditions:
    /// - A non-public instance handler named <c>On</c> with a single parameter matching the event type must exist.
    /// Behavior:
    /// - Invokes the corresponding <c>On</c> handler to mutate state, then enqueues the event for persistence/publication.
    /// </remarks>
    protected void Apply(IDomainEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        Mutate(@event);
        AddEvent(@event);
    }

    /// <summary>
    /// Invokes the appropriate non-public <c>On(TEvent)</c> method to mutate the aggregate's state.
    /// </summary>
    /// <param name="@event">The domain event used to mutate state.</param>
    /// <exception cref="InvalidOperationException">Thrown if no matching <c>On</c> handler is found.</exception>
    /// <exception cref="TargetInvocationException">Thrown if the handler itself throws.</exception>
    private void Mutate(IDomainEvent @event)
    {
        var paramTypes = new[] { @event.GetType() };
        var onMethod = GetType().GetMethod(
            name: "On",
            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: paramTypes,
            modifiers: null);

        if (onMethod is null)
        {
            throw new InvalidOperationException($"No non-public instance handler 'On({paramTypes[0].Name})' found on {GetType().Name}.");
        }

        onMethod.Invoke(this, [@event]);
    }

    /// <summary>
    /// Adds a new domain event to the internal event buffer.
    /// </summary>
    /// <param name="event">The event to record.</param>
    /// <remarks>
    /// Typically called by <see cref="Apply(IDomainEvent)"/> after state mutation. The application layer
    /// should persist and/or publish events obtained via <see cref="GetEvents"/> and then call <see cref="ClearEvents"/>.
    /// </remarks>
    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);

    /// <summary>
    /// Returns a read-only view of the events raised by this aggregate in the current unit of work.
    /// </summary>
    /// <returns>An enumerable sequence of domain events.</returns>
    public IEnumerable<IDomainEvent> GetEvents() => _events.AsEnumerable();

    /// <summary>
    /// Clears the buffered domain events. Call this after successfully persisting/publishing them.
    /// </summary>
    public void ClearEvents() => _events.Clear();
}

public abstract class AggregateRoot : AggregateRoot<long>
{

}