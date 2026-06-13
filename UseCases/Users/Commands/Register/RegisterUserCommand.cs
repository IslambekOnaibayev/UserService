namespace UseCases.Users.Commands.Register
{
    public record RegisterUserCommand(string Name, string Email, string Password)
        : Mediator.ICommand<Result<Guid>>;
}
