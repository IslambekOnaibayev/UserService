namespace Core.UserAggregate.Specifications
{
    public sealed class UnverifiedUsersSpec : Specification<User>
    {
        public UnverifiedUsersSpec()
        {
            Query.Where(user => user.Status == UserStatus.Unverified);
        }
    }
}
