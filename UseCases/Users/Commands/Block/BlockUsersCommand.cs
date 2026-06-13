namespace UseCases.Users.Commands.Block
{
    public record BlockUsersCommand(IEnumerable<Guid> UserIds) : Mediator.ICommand<Result>;
}
