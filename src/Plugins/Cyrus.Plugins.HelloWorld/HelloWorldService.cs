namespace Cyrus.Plugins.HelloWorld;

/// <summary>
/// Implementation of the HelloWorld service.
/// </summary>
public class HelloWorldService : IHelloWorldService
{
    private readonly string[] _greetings = 
    {
        "Hello from Cyrus Plugin System!",
        "Greetings from the HelloWorld Plugin!",
        "Welcome to the modular Cyrus framework!",
        "Plugin system is working perfectly!",
        "Hello, World! - From a Cyrus Plugin"
    };

    private readonly Random _random = new();

    /// <inheritdoc />
    public string GetGreeting()
    {
        var index = _random.Next(_greetings.Length);
        var greeting = _greetings[index];
        var dateTime = GetCurrentDateTime();
        
        return $"{greeting} (Generated at: {dateTime})";
    }

    /// <inheritdoc />
    public string GetPersonalizedGreeting(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return GetGreeting();

        var dateTime = GetCurrentDateTime();
        return $"Hello, {name}! Welcome to the Cyrus Plugin System! (Generated at: {dateTime})";
    }

    /// <inheritdoc />
    public string GetCurrentDateTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}