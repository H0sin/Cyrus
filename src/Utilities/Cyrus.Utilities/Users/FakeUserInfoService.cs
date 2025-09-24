using Cyrus.Utilities.Abstractions;

namespace Cyrus.Utilities.Users;

/// <summary>
/// Development/test fallback user info provider.
/// </summary>
public sealed class FakeUserInfoService : IUserInfoService
{
    private readonly string _defaultUserId;

    public FakeUserInfoService() : this("1") { }
    public FakeUserInfoService(string defaultUserId) => _defaultUserId = defaultUserId;

    public string GetUserAgent() => "Fake-Agent";
    public string GetUserIp() => "0.0.0.0";
    public string UserId() => _defaultUserId;
    public string GetFirstName() => "FirstName";
    public string GetLastName() => "LastName";
    public string GetUsername() => "Username";
    public string? GetClaim(string claimType) => claimType;
    public bool IsCurrentUser(string userId) => string.Equals(userId, _defaultUserId, StringComparison.OrdinalIgnoreCase);
    public string UserIdOrDefault() => _defaultUserId;
    public string UserIdOrDefault(string defaultValue) => string.IsNullOrEmpty(_defaultUserId) ? defaultValue : _defaultUserId;
}
