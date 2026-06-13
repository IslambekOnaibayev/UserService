namespace Core.UserAggregate.Specifications
{
    public sealed class UserListSpec : Specification<User>
    {
        public UserListSpec()
        {
            Query
            .OrderByDescending(u => u.LastLoginTime ?? u.RegistrationTime)
            .ThenBy(u => u.NormalizedEmail);
        }
    }
}
