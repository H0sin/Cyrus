using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cyrus.Core.RequestResponse.Common;
using Cyrus.Core.RequestResponse.Commands;
using Cyrus.Core.Contracts.ApplicationServices.Commands;
using Cyrus.Core.Domain.Exceptions;
using Cyrus.Utilities.Logging;
using Cyrus.Utilities.Resources;

namespace Cyrus.Core.ApplicationService.Commands;

/// <summary>
/// Decorator that catches domain exceptions thrown during command handling and
/// converts them into uniform application service results with localized messages.
/// </summary>
public class CommandDispatcherDomainExceptionHandlerDecorator : CommandDispatcherDecorator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcherDomainExceptionHandlerDecorator> _logger;

    /// <summary>
    /// Creates a new domain-exception-handling decorator.
    /// </summary>
    /// <param name="serviceProvider">Service provider used to resolve the translator.</param>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <param name="inner">The next dispatcher in the chain.</param>
    public CommandDispatcherDomainExceptionHandlerDecorator(
        IServiceProvider serviceProvider,
        ILogger<CommandDispatcherDomainExceptionHandlerDecorator> logger,
        ICommandDispatcher inner) : base(inner)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public override int Order => 2;

    /// <inheritdoc />
    public override async Task<CommandResult> Send<TCommand>(TCommand command)
    {
        try
        {
            return await CommandDispatcher.Send(command);
        }
        catch (InvalidEntityStateException ex)
        {
            _logger.LogError(CyrusEventId.DomainValidationException, ex,
                "Processing of {CommandType} With value {Command} failed at {StartDateTime} because there are domain exceptions.",
                command?.GetType(), command, DateTime.Now);
            return DomainExceptionHandlingWithoutReturnValue(ex);
        }
        catch (DomainStateException domainStateException)
        {
            _logger.LogError(CyrusEventId.DomainValidationException, domainStateException,
                "Processing of {CommandType} With value {Command} failed at {StartDateTime} because there are domain exceptions.",
                command?.GetType(), command, DateTime.Now);
            return DomainExceptionHandlingWithoutReturnValue(domainStateException);
        }
        catch
        {
            // Non-domain exceptions are not handled here; rethrow to be caught by higher-level handlers.
            throw;
        }
    }

    public override Task<CommandResult> Send<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command)
    {
        try
        {
            var result = await CommandDispatcher.Send<TCommand, TData>(command);
            return result;
        }
        catch (DomainStateException ex)
        {
            _logger.LogError(CyrusEventId.DomainValidationException, ex,
                "Processing of {CommandType} With value {Command} failed at {StartDateTime} because there are domain exceptions.",
                command?.GetType(), command, DateTime.Now);
            return DomainExceptionHandlingWithReturnValue<TData>(ex);
        }
        catch (AggregateException ex) when (ex.InnerException is DomainStateException domainStateException)
        {
            _logger.LogError(CyrusEventId.DomainValidationException, ex,
                "Processing of {CommandType} With value {Command} failed at {StartDateTime} because there are domain exceptions.",
                command?.GetType(), command, DateTime.Now);
            return DomainExceptionHandlingWithReturnValue<TData>(domainStateException);
        }
        catch
        {
            throw;
        }
    }

    public override Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private CommandResult DomainExceptionHandlingWithoutReturnValue(DomainStateException ex)
    {
        var commandResult = new CommandResult
        {
            Status = ApplicationServiceStatus.InvalidDomainState
        };
        commandResult.AddMessage(GetExceptionText(ex));
        return commandResult;
    }

    private CommandResult<TData> DomainExceptionHandlingWithReturnValue<TData>(DomainStateException ex)
    {
        var commandResult = new CommandResult<TData>
        {
            Status = ApplicationServiceStatus.InvalidDomainState
        };
        commandResult.AddMessage(GetExceptionText(ex));
        return commandResult;
    }

    private string GetExceptionText(DomainStateException domainStateException)
    {
        var translator = _serviceProvider.GetService<ITranslator>();
        if (translator == null)
            return domainStateException.ToString();

        var result = (domainStateException.Parameters?.Any() == true)
            ? translator[domainStateException.Message, domainStateException.Parameters]
            : translator[domainStateException.Message];

        _logger.LogInformation(CyrusEventId.DomainValidationException,
            "Domain Exception message is {DomainExceptionMessage}", result);
        return result;
    }
}