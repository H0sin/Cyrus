using Cyrus.Core.RequestResponse.Commands;

namespace Cyrus.Core.Contracts.ApplicationServices.Commands;

/// <summary>
/// Handles a command and returns an application-level result with a data payload.
/// </summary>
public interface ICommandHandler<TCommand, TData> where TCommand : ICommand<TData>
{
    Task<CommandResult<TData>> Handle(TCommand request);
    Task<CommandResult<TData>> Handle(TCommand request, CancellationToken cancellationToken);
}

/// <summary>
/// Handles a command and returns an application-level result.
/// </summary>
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task<CommandResult> Handle(TCommand request);
    Task<CommandResult> Handle(TCommand request, CancellationToken cancellationToken);
}
