using UseCases.Users.Commands.Unblock;

namespace WebAPI.Endpoints.Users
{
    public class UnblockUsers(IMediator mediator)
        : Endpoint<BulkUserIdsRequest, Results<NoContent, ProblemHttpResult>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Post("/api/users/unblock");
            AuthSchemes("Bearer");
            Summary(s =>
            {
                s.Summary = "Unblock selected users";
                s.Responses[204] = "Users unblocked.";
            });
            Tags("Users");
        }

        public override async Task<Results<NoContent, ProblemHttpResult>>
            ExecuteAsync(BulkUserIdsRequest req, CancellationToken ct)
        {
            await _mediator.Send(new UnblockUsersCommand(req.UserIds), ct);
            return TypedResults.NoContent();
        }
    }
}
