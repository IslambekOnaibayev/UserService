namespace Core.UserAggregate.Specifications
{
    public sealed class UsersByIdsSpec : Specification<User>
    {
        public UsersByIdsSpec(IEnumerable<Guid> ids)
        {
            var list = ids.Select(UserId.From).ToList();
            Query.Where(u => list.Contains(u.Id));
        }
    }
}
