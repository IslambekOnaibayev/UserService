namespace WebAPI.Auth
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = "UserService";
        public string Audience { get; set; } = "UserService";
        public int ExpirationMinutes { get; set; } = 60;
    }
}
