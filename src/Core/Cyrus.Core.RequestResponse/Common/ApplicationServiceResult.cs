namespace Cyrus.Core.RequestResponse.Common;

/// <summary>
/// Base type for application service results carrying a status code and optional messages.
/// </summary>
public abstract class ApplicationServiceResult : IApplicationServiceResult
{
    private readonly List<string> _messages = new();

    /// <summary>User-facing or diagnostic messages related to the operation.</summary>
    public IEnumerable<string> Messages => _messages;

    /// <summary>High-level status of the operation.</summary>
    public ApplicationServiceStatus Status { get; set; }

    /// <summary>Convenience flag indicating a successful operation.</summary>
    public bool Succeeded => Status == ApplicationServiceStatus.Ok;

    public void AddMessage(string error) => _messages.Add(error);
    public void AddMessages(IEnumerable<string> errors) => _messages.AddRange(errors);
    public void ClearMessages() => _messages.Clear();
}