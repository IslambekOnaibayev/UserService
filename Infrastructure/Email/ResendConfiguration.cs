namespace Infrastructure.Email
{
    public class ResendConfiguration
    {
        public const string SectionName = "Resend";
        public string ApiKey { get; set; } = string.Empty;
    }
}
