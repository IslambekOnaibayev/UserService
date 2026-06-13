namespace WebAPI.Endpoints.Users
{
    public record UserRecord(
        Guid Id, string Name, string Email,
        DateTimeOffset RegistrationTime,
        DateTimeOffset? LastLoginTime,
        DateTimeOffset? LastActivityTime,
        string Status);

    public record ListUsersResponse(IReadOnlyList<UserRecord> Users);
}
