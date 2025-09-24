using System.Linq;
using Cyrus.Core.RequestResponse.Commands;
using Cyrus.Core.RequestResponse.Common;
using Cyrus.Core.Contracts.ApplicationServices.Commands;
using Cyrus.Utilities.Resources;

namespace Cyrus.Core.ApplicationService.Commands;

/// <summary>
/// Base class for command handlers that return a data payload. Provides helper methods
/// to create success/failure results and to localize messages via the injected translator.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TData">The payload type returned by the command.</typeparam>
public abstract class CommandHandler<TCommand, TData> : ICommandHandler<TCommand, TData>
    where TCommand : class, ICommand<TData>
{
    protected readonly ITranslator Translator;

    protected CommandHandler(ITranslator translator)
    {
        Translator = translator;
    }

    /// <summary>
    /// Handles the command using the default cancellation token.
    /// </summary>
    public Task<CommandResult<TData>> Handle(TCommand command)
        => Handle(command, CancellationToken.None);

    /// <summary>
    /// Handles the command with a cancellation token.
    /// Implement this method in derived classes.
    /// </summary>
    public abstract Task<CommandResult<TData>> Handle(TCommand command, CancellationToken cancellationToken);

    #region Helpers (Result builders)
    protected CommandResult<TData> Ok(TData data, params string[] messageKeys)
    {
        var res = CommandResult<TData>.Ok(data);
        if (messageKeys is { Length: > 0 }) res.AddMessages(TranslateKeys(messageKeys));
        return res;
    }

    protected Task<CommandResult<TData>> OkAsync(TData data, params string[] messageKeys)
        => Task.FromResult(Ok(data, messageKeys));

    protected CommandResult<TData> Result(TData data, ApplicationServiceStatus status, params string[] messageKeys)
    {
        var res = new CommandResult<TData> { Status = status, Data = data };
        if (messageKeys is { Length: > 0 }) res.AddMessages(TranslateKeys(messageKeys));
        return res;
    }

    protected Task<CommandResult<TData>> ResultAsync(TData data, ApplicationServiceStatus status, params string[] messageKeys)
        => Task.FromResult(Result(data, status, messageKeys));

    /// <summary>Adds a localized message to the given result.</summary>
    protected void AddMessage(CommandResult<TData> result, string key)
        => result.AddMessage(Translator[key]);

    /// <summary>Adds a localized, formatted message to the given result.</summary>
    protected void AddMessage(CommandResult<TData> result, string key, params string[] arguments)
        => result.AddMessage(Translator[key, arguments]);

    /// <summary>Translates a message key using the current UI culture.</summary>
    protected string T(string key) => Translator[key];

    /// <summary>Translates and formats a message key using the current UI culture.</summary>
    protected string T(string key, params string[] args) => Translator[key, args];

    private IEnumerable<string> TranslateKeys(IEnumerable<string> keys)
        => keys.Select(k => Translator[k]);
    #endregion
}

/// <summary>
/// Base class for command handlers that do not return a data payload. Provides helper methods
/// to create success/failure results and to localize messages via the injected translator.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    protected readonly ITranslator Translator;

    protected CommandHandler(ITranslator translator)
    {
        Translator = translator;
    }

    /// <summary>
    /// Handles the command using the default cancellation token.
    /// </summary>
    public Task<CommandResult> Handle(TCommand command)
        => Handle(command, CancellationToken.None);

    /// <summary>
    /// Handles the command with a cancellation token.
    /// Implement this method in derived classes.
    /// </summary>
    public abstract Task<CommandResult> Handle(TCommand command, CancellationToken cancellationToken);

    #region Helpers (Result builders)
    protected CommandResult Ok(params string[] messageKeys)
        => CommandResult.Ok(TranslateKeys(messageKeys).ToArray());

    protected Task<CommandResult> OkAsync(params string[] messageKeys)
        => Task.FromResult(Ok(messageKeys));

    protected CommandResult Result(ApplicationServiceStatus status, params string[] messageKeys)
    {
        var res = new CommandResult { Status = status };
        if (messageKeys is { Length: > 0 }) res.AddMessages(TranslateKeys(messageKeys));
        return res;
    }

    protected Task<CommandResult> ResultAsync(ApplicationServiceStatus status, params string[] messageKeys)
        => Task.FromResult(Result(status, messageKeys));

    /// <summary>Adds a localized message to the given result.</summary>
    protected void AddMessage(CommandResult result, string key)
        => result.AddMessage(Translator[key]);

    /// <summary>Adds a localized, formatted message to the given result.</summary>
    protected void AddMessage(CommandResult result, string key, params string[] arguments)
        => result.AddMessage(Translator[key, arguments]);

    /// <summary>Translates a message key using the current UI culture.</summary>
    protected string T(string key) => Translator[key];

    /// <summary>Translates and formats a message key using the current UI culture.</summary>
    protected string T(string key, params string[] args) => Translator[key, args];

    private IEnumerable<string> TranslateKeys(IEnumerable<string> keys)
        => keys.Select(k => Translator[k]);
    
    #endregion
}