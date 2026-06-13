using UseCases.Users;
using UseCases.Users.Queries.List;

namespace WebAPI.Endpoints.Users
{
    public class ListUsers(IMediator mediator) : EndpointWithoutRequest<ListUsersResponse>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("/api/users");
            AuthSchemes("Bearer");
            Summary(s =>
            {
                s.Summary = "Get all users";
                s.Description = "Returns all users sorted by last login time. Requires authentication.";
                s.Responses[200] = "List of users.";
                s.Responses[401] = "Not authenticated.";
            });
            Tags("Users");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = await _mediator.Send(new ListUsersQuery(), ct);
            var records = result.Value.Select(ToRecord).ToList();
            await Send.OkAsync(new ListUsersResponse(records), ct);
        }

        private static UserRecord ToRecord(UserDto u) =>
            new(u.Id, u.Name, u.Email, u.RegistrationTime, u.LastLoginTime, u.LastActivityTime, u.Status);
    }
}
