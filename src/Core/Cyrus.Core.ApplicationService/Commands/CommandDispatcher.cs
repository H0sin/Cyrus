using System.Diagnostics;
using Cyrus.Core.RequestResponse.Commands;
using Cyrus.Core.Contracts.ApplicationServices.Commands;
using Cyrus.Utilities.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cyrus.Core.ApplicationService.Commands;

public class CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
    : ICommandDispatcher
{
    private readonly Stopwatch _stopwatch = new();


    #region Send Commands

    public async Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        _stopwatch.Start();
        try
        {
            logger.LogDebug("Routing command of type {CommandType} With value {Command}  Start at {StartDateTime}",
                command.GetType(), command, DateTime.Now);
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

            return await handler.Handle(command);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "There is not suitable handler for {CommandType} Routing failed at {StartDateTime}.",
                command.GetType(), DateTime.Now);
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            logger.LogInformation(CyrusEventId.PerformanceMeasurement,
                "Processing the {CommandType} command tooks {Millisecconds} Millisecconds", command.GetType(),
                _stopwatch.ElapsedMilliseconds);
        }
    }

    public async Task<CommandResult> Send<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand
    {
        _stopwatch.Start();

        try
        {
            logger.LogDebug("Routing command of type {CommandType} With value {Command}  Start at {StartDateTime}",
                command.GetType(), command, DateTime.Now);
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

            return await handler.Handle(command, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "There is not suitable handler for {CommandType} Routing failed at {StartDateTime}.",
                command.GetType(), DateTime.Now);
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            logger.LogInformation(CyrusEventId.PerformanceMeasurement,
                "Processing the {CommandType} command tooks {Millisecconds} Millisecconds", command.GetType(),
                _stopwatch.ElapsedMilliseconds);
        }
    }

    public async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command)
        where TCommand : class, ICommand<TData>
    {
        _stopwatch.Start();
        try
        {
            logger.LogDebug("Routing command of type {CommandType} With value {Command}  Start at {StartDateTime}",
                command.GetType(), command, DateTime.Now);
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TData>>();
            return await handler.Handle(command);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "There is not suitable handler for {CommandType} Routing failed at {StartDateTime}.",
                command.GetType(), DateTime.Now);
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            logger.LogInformation("Processing the {CommandType} command tooks {Millisecconds} Millisecconds",
                command.GetType(), _stopwatch.ElapsedMilliseconds);
        }
    }

    public async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TData>
    {
        _stopwatch.Start();
        try
        {
            logger.LogDebug("Routing command of type {CommandType} With value {Command}  Start at {StartDateTime}",
                command.GetType(), command, DateTime.Now);
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TData>>();
            
            return await handler.Handle(command, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "There is not suitable handler for {CommandType} Routing failed at {StartDateTime}.",
                command.GetType(), DateTime.Now);
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            logger.LogInformation("Processing the {CommandType} command tooks {Millisecconds} Millisecconds",
                command.GetType(), _stopwatch.ElapsedMilliseconds);
        }
    }

    #endregion
}