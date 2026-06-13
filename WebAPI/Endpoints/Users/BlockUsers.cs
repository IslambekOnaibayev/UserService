using UseCases.Users.Commands.Block;

namespace WebAPI.Endpoints.Users
{
    public class BlockUsers(IMediator mediator)
    : Endpoint<BulkUserIdsRequest, Results<NoContent, ProblemHttpResult>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Post("/api/users/block");
            AuthSchemes("Bearer");
            Summary(s =>
            {
                s.Summary = "Block selected users";
                s.Description = "Blocks one or more users. Blocked users cannot login.";
                s.Responses[204] = "Users blocked.";
            });
            Tags("Users");
        }

        public override async Task<Results<NoContent, ProblemHttpResult>>
            ExecuteAsync(BulkUserIdsRequest req, CancellationToken ct)
        {
            await _mediator.Send(new BlockUsersCommand(req.UserIds), ct);
            return TypedResults.NoContent();
        }
    }
}
