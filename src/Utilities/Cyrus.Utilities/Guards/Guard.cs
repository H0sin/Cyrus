namespace Cyrus.Utilities.Guards;

/// <summary>
/// Entry point for guard clause extensions. Use <c>Guard.ThrowIf</c> then call extension methods
/// like <c>Guard.ThrowIf.Empty(value, "message")</c>.
/// </summary>
public sealed class Guard
{
    /// <summary>Singleton entry for guard clauses.</summary>
    public static Guard ThrowIf { get; } = new();

    private Guard() { }
}
