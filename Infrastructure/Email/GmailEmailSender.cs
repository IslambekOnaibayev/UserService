using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email
{
    public class GmailEmailSender(
        IHttpClientFactory httpClientFactory,
        IOptions<GmailConfiguration> gmailOptions,
        IOptions<MailserverConfiguration> mailOptions,
        ILogger<GmailEmailSender> logger) : IEmailSender
    {
        private readonly GmailConfiguration _gmail = gmailOptions.Value;
        private readonly MailserverConfiguration _mail = mailOptions.Value;

        public async Task SendConfirmationEmailAsync(
            string toEmail, Guid userId, string token, CancellationToken cancellationToken = default)
        {
            logger.LogWarning(
                "Sending confirmation email to {Email} using {Type}", toEmail, nameof(GmailEmailSender));

            var confirmUrl =
                $"{_mail.BaseUrl}/api/users/confirm-email" +
                $"?userId={userId}&token={Uri.EscapeDataString(token)}";

            var accessToken = await RefreshAccessTokenAsync(cancellationToken);

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_mail.FromName, _mail.FromEmail));
            mimeMessage.To.Add(new MailboxAddress(toEmail, toEmail));
            mimeMessage.Subject = "Confirm your email address";
            mimeMessage.Body = new TextPart("html")
            {
                Text = $"""
                    <p>Click the link below to confirm your email address:</p>
                    <p><a href="{confirmUrl}">Confirm Email</a></p>
                    <p>If you did not create an account, you can ignore this email.</p>
                    """
            };

            using var ms = new MemoryStream();
            await mimeMessage.WriteToAsync(ms, cancellationToken);
            var raw = Convert.ToBase64String(ms.ToArray())
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');

            var client = httpClientFactory.CreateClient("gmail");
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://gmail.googleapis.com/gmail/v1/users/me/messages/send");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(new { raw });

            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("Gmail API error {Status}: {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Failed to send email via Gmail API: {response.StatusCode}");
            }

            logger.LogInformation("Confirmation email sent via Gmail API to {Email}", toEmail);
        }

        private async Task<string> RefreshAccessTokenAsync(CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient("gmail");
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _gmail.ClientId,
                ["client_secret"] = _gmail.ClientSecret,
                ["refresh_token"] = _gmail.RefreshToken,
                ["grant_type"] = "refresh_token"
            });

            var response = await client.PostAsync(
                "https://oauth2.googleapis.com/token", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("Gmail token refresh failed: {Error}", error);
                throw new InvalidOperationException("Failed to refresh Gmail OAuth2 token");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(
                cancellationToken: cancellationToken);

            return json.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("No access_token in Gmail token response");
        }
    }
}
