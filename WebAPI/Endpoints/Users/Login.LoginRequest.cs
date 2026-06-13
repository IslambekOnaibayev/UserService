using System.ComponentModel.DataAnnotations;

namespace WebAPI.Endpoints.Users
{
    public class LoginRequest
    {
        public const string Route = "/api/auth/login";

        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
}
