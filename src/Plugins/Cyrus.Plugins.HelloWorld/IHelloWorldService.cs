namespace Cyrus.Plugins.HelloWorld;

/// <summary>
/// Service interface for HelloWorld functionality.
/// </summary>
public interface IHelloWorldService
{
    /// <summary>
    /// Gets a greeting message.
    /// </summary>
    /// <returns>A greeting message.</returns>
    string GetGreeting();

    /// <summary>
    /// Gets a personalized greeting message.
    /// </summary>
    /// <param name="name">The name to include in the greeting.</param>
    /// <returns>A personalized greeting message.</returns>
    string GetPersonalizedGreeting(string name);

    /// <summary>
    /// Gets the current date and time formatted as a string.
    /// </summary>
    /// <returns>The current date and time.</returns>
    string GetCurrentDateTime();
}