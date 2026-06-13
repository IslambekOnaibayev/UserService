namespace UseCases.Users.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(Guid UserId, string Token)
        : Mediator.ICommand<Result>;
}
