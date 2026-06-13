namespace UseCases.Users.Commands.Block
{
    public class BlockUsersHandler(IRepository<User> repository) : ICommandHandler<BlockUsersCommand, Result>
    {
        private readonly IRepository<User> _repository = repository;

        public async ValueTask<Result> Handle(BlockUsersCommand command, CancellationToken cancellationToken)
        {
            var users = await _repository.ListAsync(new UsersByIdsSpec(command.UserIds), cancellationToken);
            foreach (var user in users) user.Block();
            await _repository.UpdateRangeAsync(users, cancellationToken);
            return Result.Success();
        }
    }
}
