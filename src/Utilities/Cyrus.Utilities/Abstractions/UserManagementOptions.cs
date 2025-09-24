namespace Cyrus.Utilities.Abstractions;

public sealed class UserManagementOptions
{
    public string DefaultUserAgent { get; set; } = "Unknown-Agent";
    public string DefaultUserIp { get; set; } = "0.0.0.0";
    public string DefaultUserId { get; set; } = "1";
    public string DefaultFirstName { get; set; } = "FirstName";
    public string DefaultLastName { get; set; } = "LastName";
    public string DefaultUsername { get; set; } = "Username";
}

