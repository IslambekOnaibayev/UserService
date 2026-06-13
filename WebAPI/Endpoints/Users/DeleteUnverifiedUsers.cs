using UseCases.Users.Commands.DeleteUnverified;

namespace WebAPI.Endpoints.Users
{
    public class DeleteUnverifiedUsers(IMediator mediator)
    : EndpointWithoutRequest<Results<NoContent, ProblemHttpResult>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Delete("/api/users/unverified");
            AuthSchemes("Bearer");
            Summary(s =>
            {
                s.Summary = "Delete all unverified users";
                s.Description = "Permanently deletes all users with Unverified status.";
                s.Responses[204] = "Unverified users deleted.";
            });
            Tags("Users");
        }

        public override async Task<Results<NoContent, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
        {
            await _mediator.Send(new DeleteUnverifiedUsersCommand(), ct);
            return TypedResults.NoContent();
        }
    }
}
