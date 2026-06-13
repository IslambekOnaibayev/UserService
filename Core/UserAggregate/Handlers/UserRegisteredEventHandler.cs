using Core.Interfaces;
using Core.UserAggregate.Events;
using Microsoft.Extensions.Logging;

namespace Core.UserAggregate.Handlers
{
    public class UserRegisteredEventHandler(IEmailSender emailSender, ILogger<UserRegisteredEventHandler> logger)
    : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<UserRegisteredEventHandler> _logger = logger;

        public async ValueTask Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Sending confirmation email to {Email} for UserId {UserId}",
                notification.Email.Value, notification.UserId.Value);

            await _emailSender.SendConfirmationEmailAsync(
                notification.Email.Value,
                notification.UserId.Value,
                notification.Token.Value,
                cancellationToken);
        }
    }
}
