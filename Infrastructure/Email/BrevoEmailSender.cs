using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email
{
    public class BrevoEmailSender(
        IHttpClientFactory httpClientFactory,
        IOptions<BrevoConfiguration> brevoOptions,
        IOptions<MailserverConfiguration> mailOptions,
        ILogger<BrevoEmailSender> logger) : IEmailSender
    {
        private readonly string _apiKey = brevoOptions.Value.ApiKey;
        private readonly MailserverConfiguration _mail = mailOptions.Value;

        public async Task SendConfirmationEmailAsync(
            string toEmail, Guid userId, string token, CancellationToken cancellationToken = default)
        {
            logger.LogWarning(
                "Sending confirmation email to {Email} using {Type}", toEmail, nameof(BrevoEmailSender));

            var confirmUrl =
                $"{_mail.BaseUrl}/api/users/confirm-email" +
                $"?userId={userId}&token={Uri.EscapeDataString(token)}";

            var client = httpClientFactory.CreateClient("brevo");

            var payload = new
            {
                sender = new { name = _mail.FromName, email = _mail.FromEmail },
                to = new[] { new { email = toEmail } },
                subject = "Confirm your email address",
                htmlContent = $"""
                    <p>Click the link below to confirm your email address:</p>
                    <p><a href="{confirmUrl}">Confirm Email</a></p>
                    <p>If you did not create an account, you can ignore this email.</p>
                    """
            };

            var response = await client.PostAsJsonAsync(
                "https://api.brevo.com/v3/smtp/email", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("Brevo API error {Status}: {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Failed to send email via Brevo: {response.StatusCode}");
            }

            logger.LogInformation("Confirmation email sent via Brevo to {Email}", toEmail);
        }
    }
}
