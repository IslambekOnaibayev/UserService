namespace UseCases.Users.Commands.Delete
{
    public record DeleteUsersCommand(IEnumerable<Guid> UserIds) : Mediator.ICommand<Result>;
}
