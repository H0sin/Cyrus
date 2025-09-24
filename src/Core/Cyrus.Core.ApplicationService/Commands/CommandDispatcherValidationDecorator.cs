using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Cyrus.Core.RequestResponse.Common;
using Cyrus.Core.RequestResponse.Commands;
using Cyrus.Core.Contracts.ApplicationServices.Commands;
using Cyrus.Utilities.Logging;

namespace Cyrus.Core.ApplicationService.Commands;

/// <summary>
/// Decorator that performs FluentValidation on commands before dispatching them to the actual handler.
/// Logs validation attempts and outcomes with CyrusEventId.CommandValidation.
/// </summary>
public class CommandDispatcherValidationDecorator : CommandDispatcherDecorator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcherValidationDecorator> _logger;

    /// <summary>
    /// Initializes a new instance of the validation decorator.
    /// </summary>
    /// <param name="serviceProvider">Service provider used to resolve validators.</param>
    /// <param name="logger">Logger for validation diagnostics.</param>
    /// <param name="inner">The next dispatcher in the chain.</param>
    public CommandDispatcherValidationDecorator(
        IServiceProvider serviceProvider,
        ILogger<CommandDispatcherValidationDecorator> logger,
        ICommandDispatcher inner) : base(inner)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public override int Order => 1;

    /// <inheritdoc />
    public override async Task<CommandResult> Send<TCommand>(TCommand command)
    {
        _logger.LogDebug(CyrusEventId.CommandValidation,
            "Validating command of type {CommandType} With value {Command}  start at :{StartDateTime}",
            command.GetType(), command, DateTime.Now);
        var validationResult = Validate<TCommand, CommandResult>(command);

        if (validationResult != null)
        {
            _logger.LogInformation(CyrusEventId.CommandValidation,
                "Validating command of type {CommandType} With value {Command}  failed. Validation errors are: {ValidationErrors}",
                command.GetType(), command, validationResult.Messages);
            return validationResult;
        }

        _logger.LogDebug(CyrusEventId.CommandValidation,
            "Validating command of type {CommandType} With value {Command}  finished at :{EndDateTime}",
            command.GetType(), command, DateTime.Now);
        return await CommandDispatcher.Send(command);
    }

    /// <inheritdoc />
    public override async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command)
    {
        _logger.LogDebug(CyrusEventId.CommandValidation,
            "Validating command of type {CommandType} With value {Command}  start at :{StartDateTime}",
            command.GetType(), command, DateTime.Now);

        var validationResult = Validate<TCommand, CommandResult<TData>>(command);

        if (validationResult != null)
        {
            _logger.LogInformation(CyrusEventId.CommandValidation,
                "Validating command of type {CommandType} With value {Command}  failed. Validation errors are: {ValidationErrors}",
                command.GetType(), command, validationResult.Messages);
            return validationResult;
        }

        _logger.LogDebug(CyrusEventId.CommandValidation,
            "Validating command of type {CommandType} With value {Command}  finished at :{EndDateTime}",
            command.GetType(), command, DateTime.Now);
        return await CommandDispatcher.Send<TCommand, TData>(command);
    }

    /// <inheritdoc />
    public override async Task<CommandResult> Send<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug(CyrusEventId.CommandValidation,
            "Validating command of type {CommandType} With value {Command}  start at :{StartDateTime}",
            command.GetType(), command, DateTime.Now);
        var validationResult = Validate<TCommand, CommandResult>(command);
        if (validationResult != null)
        {
            _logger.LogInformation(CyrusEventId.CommandValidation,
                "Validating command of type {CommandType} With value {Command}  failed. Validation errors are: {ValidationErrors}",
                command.GetType(), command, validationResult.Messages);
            return validationResult;
        }

        _logger.LogDebug(CyrusEventId.CommandValidation,
            "Validating command of type {CommandType} With value {Command}  finished at :{EndDateTime}",
            command.GetType(), command, DateTime.Now);
        return await CommandDispatcher.Send(command, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<CommandResult<TData>> Send<TCommand, TData>(TCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(CyrusEventId.CommandValidation,
            "Validating command of type {CommandType} With value {Command}  start at :{StartDateTime}",
            command.GetType(), command, DateTime.Now);
        var validationResult = Validate<TCommand, CommandResult<TData>>(command);
        if (validationResult != null)
        {
            _logger.LogInformation(CyrusEventId.CommandValidation,
                "Validating command of type {CommandType} With value {Command}  failed. Validation errors are: {ValidationErrors}",
                command.GetType(), command, validationResult.Messages);
            return validationResult;
        }

        _logger.LogDebug(CyrusEventId.CommandValidation,
            "Validating command of type {CommandType} With value {Command}  finished at :{EndDateTime}",
            command.GetType(), command, DateTime.Now);
        return await CommandDispatcher.Send<TCommand, TData>(command, cancellationToken);
    }

    /// <summary>
    /// Resolves and executes a validator for the given command type; returns a populated result on failure, otherwise null.
    /// </summary>
    private TValidationResult? Validate<TCommand, TValidationResult>(TCommand command)
        where TValidationResult : ApplicationServiceResult, new()
    {
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();
        TValidationResult? res = null;

        if (validator != null)
        {
            var validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
            {
                res = new()
                {
                    Status = ApplicationServiceStatus.ValidationError
                };
                foreach (var item in validationResult.Errors)
                {
                    res.AddMessage(item.ErrorMessage);
                }
            }
        }
        else
        {
            _logger.LogInformation(CyrusEventId.CommandValidation, "There is not any validator for {CommandType}",
                command?.GetType());
        }

        return res;
    }
}