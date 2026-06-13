namespace UseCases.Users.Commands.Unblock
{
    public class UnblockUsersHandler(IRepository<User> repository) : ICommandHandler<UnblockUsersCommand, Result>
    {
        private readonly IRepository<User> _repository = repository;

        public async ValueTask<Result> Handle(UnblockUsersCommand command, CancellationToken cancellationToken)
        {
            var users = await _repository.ListAsync(new UsersByIdsSpec(command.UserIds), cancellationToken);
            foreach (var user in users) user.Unblock();
            await _repository.UpdateRangeAsync(users, cancellationToken);
            return Result.Success();
        }
    }
}
