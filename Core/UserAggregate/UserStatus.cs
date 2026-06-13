namespace Core.UserAggregate
{
    public sealed class UserStatus : SmartEnum<UserStatus>
    {
        public static readonly UserStatus Unverified = new(nameof(Unverified), 1);
        public static readonly UserStatus Active = new(nameof(Active), 2);
        public static readonly UserStatus Blocked = new(nameof(Blocked), 3);

        private UserStatus(string name, int value) : base(name, value) { }
    }
}
