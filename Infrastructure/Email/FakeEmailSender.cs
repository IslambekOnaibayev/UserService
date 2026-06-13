namespace Infrastructure.Email
{
    public class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailSender
    {
        private readonly ILogger<FakeEmailSender> _logger = logger;

        public Task SendConfirmationEmailAsync(
            string toEmail, Guid userId, string token, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Not actually sending confirmation email to {Email} (userId={UserId}) with token {Token}",
                toEmail, userId, token);
            return Task.CompletedTask;
        }
    }
}
