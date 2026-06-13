namespace UseCases.Users.Commands.Login
{
    public record LoginUserCommand(string Email, string Password)
        : Mediator.ICommand<Result<Guid>>;
}
