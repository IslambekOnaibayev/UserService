namespace WebAPI.Endpoints.Users
{
    public class BulkUserIdsRequest
    {
        public IEnumerable<Guid> UserIds { get; set; } = Enumerable.Empty<Guid>();
    }
}
