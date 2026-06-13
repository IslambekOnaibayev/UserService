using Microsoft.AspNetCore.Identity.Data;
using WebAPI.Extensions;

namespace WebAPI.Endpoints.Users
{
    public class Register(IMediator mediator)
        : Endpoint<RegisterRequest, Results<Created<RegisterResponse>, ValidationProblem, ProblemHttpResult>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Post(RegisterRequest.Route);
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Register a new user";
                s.Description = "Creates a new user account and sends a confirmation email asynchronously.";
                s.ExampleRequest = new RegisterRequest { Name = "John Doe", Email = "john@example.com", Password = "secret" };
                s.Responses[201] = "User registered successfully.";
                s.Responses[400] = "Validation errors.";
                s.Responses[409] = "Email already taken.";
            });
            Tags("Auth");
        }

        public override async Task<Results<Created<RegisterResponse>, ValidationProblem, ProblemHttpResult>>
            ExecuteAsync(RegisterRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(
                new RegisterUserCommand(request.Name, request.Email, request.Password), ct);

            return result.ToCreatedResult(
                id => $"/api/users/{id}",
                id => new RegisterResponse(id, "Registration successful. Please check your email."));
        }
    }
}
