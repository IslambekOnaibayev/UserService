namespace Core.UserAggregate
{
    public sealed class User : EntityBase<User, UserId>, IAggregateRoot
    {
        public UserName Name { get; private set; }
        public UserEmail Email { get; private set; }
        public NormalizedEmail NormalizedEmail { get; private set; }
        public PasswordHash PasswordHash { get; private set; }
        public DateTimeOffset RegistrationTime { get; private set; }
        public DateTimeOffset? LastLoginTime { get; private set; }
        public DateTimeOffset? LastActivityTime { get; private set; }
        public UserStatus Status { get; private set; } = UserStatus.Unverified;
        public EmailConfirmationToken? EmailConfirmationToken { get; private set; }

        public bool IsBlocked => Status == UserStatus.Blocked;

        private User() { }

        private User(UserName name, UserEmail email, PasswordHash passwordHash)
        {
            Id = UserId.New();
            Name = name;
            Email = email;
            NormalizedEmail = NormalizedEmail.FromEmail(email);
            PasswordHash = passwordHash;
            RegistrationTime = DateTimeOffset.UtcNow;
            Status = UserStatus.Unverified;
            EmailConfirmationToken = ValueObjects.EmailConfirmationToken.New();
        }

        public static User Register(string name, string email, string passwordHash)
        {
            var user = new User(
                UserName.From(name),
                UserEmail.From(email),
                PasswordHash.From(passwordHash));

            user.RegisterDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.EmailConfirmationToken!.Value));
            return user;
        }

        public void ConfirmEmail(string token)
        {
            Guard.Against.NullOrWhiteSpace(token);

            if (Status == UserStatus.Blocked) return;

            if (EmailConfirmationToken is null ||
                !StringComparer.Ordinal.Equals(EmailConfirmationToken.Value.Value, token))
                throw new InvalidOperationException("Invalid email confirmation token.");

            Status = UserStatus.Active;
            EmailConfirmationToken = null;
        }

        public void MarkLoggedIn(DateTimeOffset now)
        {
            if (IsBlocked) 
                throw new InvalidOperationException("Blocked user cannot login.");
            
            LastLoginTime = now;
            LastActivityTime = now;
        }

        public void Touch(DateTimeOffset now) => LastActivityTime = now;

        public void Block() => Status = UserStatus.Blocked;

        public void Unblock()
        {
            if (Status == UserStatus.Blocked)
                Status = EmailConfirmationToken is null ? UserStatus.Active : UserStatus.Unverified;
        }

        public void RegenerateConfirmationToken()
        {
            EmailConfirmationToken = ValueObjects.EmailConfirmationToken.New();
            RegisterDomainEvent(new UserRegisteredEvent(Id, Email, EmailConfirmationToken.Value));
        }

        public static string NormalizeEmail(string email) =>
            Guard.Against.NullOrWhiteSpace(email).Trim().ToUpperInvariant();
    }
}
