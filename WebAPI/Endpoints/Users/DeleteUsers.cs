using UseCases.Users.Commands.Delete;

namespace WebAPI.Endpoints.Users
{
    public class DeleteUsers(IMediator mediator)
        : Endpoint<BulkUserIdsRequest, Results<NoContent, ProblemHttpResult>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Delete("/api/users");
            AuthSchemes("Bearer");
            Summary(s =>
            {
                s.Summary = "Delete selected users";
                s.Description = "Permanently deletes selected users. Deleted users can re-register.";
                s.Responses[204] = "Users deleted.";
            });
            Tags("Users");
        }

        public override async Task<Results<NoContent, ProblemHttpResult>>
            ExecuteAsync(BulkUserIdsRequest req, CancellationToken ct)
        {
            await _mediator.Send(new DeleteUsersCommand(req.UserIds), ct);
            return TypedResults.NoContent();
        }
    }
}
