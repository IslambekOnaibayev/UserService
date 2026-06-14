namespace Infrastructure.Email
{
    public class SendGridConfiguration
    {
        public const string SectionName = "SendGrid";
        public string ApiKey { get; set; } = string.Empty;
    }
}
