namespace Core.UserAggregate.Specifications
{
    public sealed class UserByEmailSpec : SingleResultSpecification<User>
    {
        public UserByEmailSpec(string email)
        {
            var normalized = NormalizedEmail.From(User.NormalizeEmail(email));
            Query.Where(u => u.NormalizedEmail == normalized);
        }
    }
}
