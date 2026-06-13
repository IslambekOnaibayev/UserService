using System.ComponentModel.DataAnnotations;

namespace WebAPI.Endpoints.Users
{
    public class RegisterRequest
    {
        public const string Route = "/api/auth/register";

        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
}
