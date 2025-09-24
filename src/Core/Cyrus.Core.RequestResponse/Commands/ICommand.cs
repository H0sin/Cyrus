namespace Cyrus.Core.RequestResponse.Commands;

/// <summary>
/// Marker interface for commands that represent intent to change application state (create/update/delete).
/// </summary>
public interface ICommand;

/// <summary>
/// Marker interface for commands that expect a data payload as a result (e.g., newly created identifier).
/// </summary>
/// <typeparam name="TData">Type of the expected result payload.</typeparam>
public interface ICommand<TData>;