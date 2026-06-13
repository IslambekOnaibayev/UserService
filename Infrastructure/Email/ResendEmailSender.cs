using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email
{
    public class ResendEmailSender(
        IHttpClientFactory httpClientFactory,
        IOptions<ResendConfiguration> resendOptions,
        IOptions<MailserverConfiguration> mailOptions,
        ILogger<ResendEmailSender> logger) : IEmailSender
    {
        private readonly string _apiKey = resendOptions.Value.ApiKey;
        private readonly MailserverConfiguration _mail = mailOptions.Value;

        public async Task SendConfirmationEmailAsync(
            string toEmail, Guid userId, string token, CancellationToken cancellationToken = default)
        {
            var confirmUrl =
                $"{_mail.BaseUrl}/api/users/confirm-email" +
                $"?userId={userId}&token={Uri.EscapeDataString(token)}";

            var client = httpClientFactory.CreateClient("resend");

            var payload = new
            {
                from = $"{_mail.FromName} <{_mail.FromEmail}>",
                to = new[] { toEmail },
                subject = "Confirm your email address",
                html = $"""
                    <p>Click the link below to confirm your email address:</p>
                    <p><a href="{confirmUrl}">Confirm Email</a></p>
                    <p>If you did not create an account, you can ignore this email.</p>
                    """
            };

            var response = await client.PostAsJsonAsync(
                "https://api.resend.com/emails", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("Resend API error {Status}: {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Failed to send email via Resend: {response.StatusCode}");
            }

            logger.LogInformation("Confirmation email sent via Resend to {Email}", toEmail);
        }
    }
}
