using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email
{
    public class SendGridEmailSender(
        IOptions<SendGridConfiguration> sendGridOptions,
        IOptions<MailserverConfiguration> mailOptions,
        ILogger<SendGridEmailSender> logger) : IEmailSender
    {
        private readonly string _apiKey = sendGridOptions.Value.ApiKey;
        private readonly MailserverConfiguration _mail = mailOptions.Value;

        public async Task SendConfirmationEmailAsync(
            string toEmail, Guid userId, string token, CancellationToken cancellationToken = default)
        {
            logger.LogWarning(
                "Sending confirmation email to {Email} using {Type}", toEmail, nameof(SendGridEmailSender));

            var confirmUrl =
                $"{_mail.BaseUrl}/api/users/confirm-email" +
                $"?userId={userId}&token={Uri.EscapeDataString(token)}";

            var client = new SendGridClient(_apiKey);
            var msg = MailHelper.CreateSingleEmail(
                from: new EmailAddress(_mail.FromEmail, _mail.FromName),
                to: new EmailAddress(toEmail),
                subject: "Confirm your email address",
                plainTextContent: $"Click the link to confirm your email: {confirmUrl}",
                htmlContent: $"""
                    <p>Click the link below to confirm your email address:</p>
                    <p><a href="{confirmUrl}">Confirm Email</a></p>
                    <p>If you did not create an account, you can ignore this email.</p>
                    """);

            var response = await client.SendEmailAsync(msg, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync(cancellationToken);
                logger.LogError("SendGrid API error {Status}: {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Failed to send email via SendGrid: {response.StatusCode}");
            }

            logger.LogInformation("Confirmation email sent via SendGrid to {Email}", toEmail);
        }
    }
}
