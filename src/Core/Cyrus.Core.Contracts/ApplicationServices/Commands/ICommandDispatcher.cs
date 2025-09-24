using Cyrus.Core.RequestResponse.Commands;

namespace Cyrus.Core.Contracts.ApplicationServices.Commands;

/// <summary>
/// Mediator-style dispatcher for commands. Routes a command to its appropriate handler.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Sends a command to its handler and returns an execution result.
    /// </summary>
    Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : class, ICommand;

    /// <summary>
    /// Sends a command to its handler and returns an execution result.
    /// </summary>
    Task<CommandResult> Send<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand;

    /// <summary>
    /// Sends a command to its handler and returns an execution result with a data payload.
    /// </summary>
    Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command) where TCommand : class, ICommand<TData>;

    /// <summary>
    /// Sends a command to its handler and returns an execution result with a data payload.
    /// </summary>
    Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand<TData>;
}
