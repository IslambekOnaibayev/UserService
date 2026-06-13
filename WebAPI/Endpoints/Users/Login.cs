using Microsoft.AspNetCore.Identity.Data;
using UseCases.Users.Commands.Login;

namespace WebAPI.Endpoints.Users
{
    public class Login(IMediator mediator, ITokenService tokenService)
        : Endpoint<LoginRequest, Results<Ok<LoginResponse>, ProblemHttpResult>>
    {
        private readonly IMediator _mediator = mediator;
        private readonly ITokenService _tokenService = tokenService;

        public override void Configure()
        {
            Post(LoginRequest.Route);
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Login";
                s.Description = "Authenticates a user and returns a JWT token. Blocked users cannot login.";
                s.Responses[200] = "Login successful.";
                s.Responses[401] = "Invalid credentials.";
                s.Responses[403] = "Account is blocked.";
            });
            Tags("Auth");
        }

        public override async Task<Results<Ok<LoginResponse>, ProblemHttpResult>>
            ExecuteAsync(LoginRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new LoginUserCommand(request.Email, request.Password), ct);

            if (result.Status == ResultStatus.Forbidden)
                return TypedResults.Problem(
                    title: "Forbidden",
                    detail: "Your account has been blocked.",
                    statusCode: StatusCodes.Status403Forbidden);

            if (!result.IsSuccess)
                return TypedResults.Problem(
                    title: "Unauthorized",
                    detail: "Invalid email or password.",
                    statusCode: StatusCodes.Status401Unauthorized);

            var token = _tokenService.GenerateToken(result.Value, request.Email);
            return TypedResults.Ok(new LoginResponse(token, result.Value));
        }
    }
}
