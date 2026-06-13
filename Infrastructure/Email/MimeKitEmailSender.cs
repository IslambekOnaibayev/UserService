using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email
{
    public class MimeKitEmailSender(
    ILogger<MimeKitEmailSender> logger,
    IOptions<MailserverConfiguration> options) : IEmailSender
    {
        private readonly ILogger<MimeKitEmailSender> _logger = logger;
        private readonly MailserverConfiguration _config = options.Value;

        public async Task SendConfirmationEmailAsync(
            string toEmail, Guid userId, string token, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning(
                "Sending confirmation email to {Email} using {Type}", toEmail, nameof(MimeKitEmailSender));

            var confirmUrl = $"{_config.BaseUrl}/api/users/confirm-email?userId={userId}&token={Uri.EscapeDataString(token)}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config.FromName, _config.FromEmail));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = "Confirm your email address";
            message.Body = new TextPart("html")
            {
                Text = $"<p>Click to confirm your email: <a href=\"{confirmUrl}\">Confirm Email</a></p>"
            };

            try
            {
                using var client = new SmtpClient();
                var socketOptions = _config.UseSsl
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTlsWhenAvailable;

                await client.ConnectAsync(_config.Hostname, _config.Port, socketOptions, cancellationToken);

                if (!string.IsNullOrEmpty(_config.Username) && !string.IsNullOrEmpty(_config.Password))
                    await client.AuthenticateAsync(_config.Username, _config.Password, cancellationToken);

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, new CancellationToken(canceled: true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", toEmail);
                throw;
            }
        }
    }
}
