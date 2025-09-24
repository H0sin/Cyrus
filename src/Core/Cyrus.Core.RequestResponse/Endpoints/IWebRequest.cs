namespace Cyrus.Core.RequestResponse.Endpoints;

/// <summary>
/// Contract representing a web endpoint request that maps to a specific path.
/// </summary>
public interface IWebRequest
{
    /// <summary>Request path for routing purposes.</summary>
    string Path { get; }
}
