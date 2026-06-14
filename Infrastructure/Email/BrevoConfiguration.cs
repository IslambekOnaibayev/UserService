namespace Infrastructure.Email
{
    public class BrevoConfiguration
    {
        public const string SectionName = "Brevo";
        public string ApiKey { get; set; } = string.Empty;
    }
}
