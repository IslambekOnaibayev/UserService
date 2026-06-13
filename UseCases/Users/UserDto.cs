namespace UseCases.Users
{
    public record UserDto(
        Guid Id,
        string Name,
        string Email,
        DateTimeOffset RegistrationTime,
        DateTimeOffset? LastLoginTime,
        DateTimeOffset? LastActivityTime,
        string Status);
}
