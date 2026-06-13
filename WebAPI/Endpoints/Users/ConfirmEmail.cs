using UseCases.Users.Commands.ConfirmEmail;

namespace WebAPI.Endpoints.Users
{
    public class ConfirmEmail(IMediator mediator) : Endpoint<ConfirmEmailRequest>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get(ConfirmEmailRequest.Route);
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Confirm email address";
                s.Description = "Confirms user email using the token from the confirmation email link.";
                s.Responses[200] = "Email confirmed.";
                s.Responses[400] = "Invalid token.";
            });
            Tags("Users");
        }

        public override async Task HandleAsync(ConfirmEmailRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(request.UserId, request.Token), ct);

            if (!result.IsSuccess)
            {
                await Send.RedirectAsync("/login?confirmed=false", isPermanent: false);
                return;
            }

            await Send.RedirectAsync("/login?confirmed=true", isPermanent: false);
        }
    }
}
