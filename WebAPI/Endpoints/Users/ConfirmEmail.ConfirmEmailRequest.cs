namespace WebAPI.Endpoints.Users
{
    public class ConfirmEmailRequest
    {
        public const string Route = "/api/users/confirm-email";

        [QueryParam] public Guid UserId { get; init; }
        [QueryParam] public string Token { get; init; } = string.Empty;
    }
}
