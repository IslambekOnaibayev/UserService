namespace Infrastructure.Email
{
    public class GmailConfiguration
    {
        public const string SectionName = "Gmail";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
