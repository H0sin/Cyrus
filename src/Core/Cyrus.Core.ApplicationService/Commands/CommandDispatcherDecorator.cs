using Cyrus.Core.RequestResponse.Commands;
using Cyrus.Core.Contracts.ApplicationServices.Commands;

namespace Cyrus.Core.ApplicationService.Commands;

public abstract class CommandDispatcherDecorator(ICommandDispatcher commandDispatcher) : ICommandDispatcher
{
    protected ICommandDispatcher CommandDispatcher = commandDispatcher;
    public abstract int Order { get; }
    
    public void SetCommandDispatcher(ICommandDispatcher commandDispatcher)
    {
        CommandDispatcher = commandDispatcher;
    }
    public abstract Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : class, ICommand;

    public abstract Task<CommandResult> Send<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand;

    public abstract Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command)
        where TCommand : class, ICommand<TData>;

    public abstract Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command,
        CancellationToken cancellationToken) where TCommand : class, ICommand<TData>;
}