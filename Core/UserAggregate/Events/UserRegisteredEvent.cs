namespace Core.UserAggregate.Events
{
    public sealed class UserRegisteredEvent(UserId userId, UserEmail email, EmailConfirmationToken token)
        : DomainEventBase
    {
        public UserId UserId { get; } = userId;
        public UserEmail Email { get; } = email;
        public EmailConfirmationToken Token { get; } = token;
    }
}
