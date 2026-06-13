namespace UseCases.Users.Commands.Unblock
{
    public record UnblockUsersCommand(IEnumerable<Guid> UserIds) : Mediator.ICommand<Result>;
}
