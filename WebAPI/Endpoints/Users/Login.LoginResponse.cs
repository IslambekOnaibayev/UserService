namespace WebAPI.Endpoints.Users
{
    public record LoginResponse(string Token, Guid UserId);
}
