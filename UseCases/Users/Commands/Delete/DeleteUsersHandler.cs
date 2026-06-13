namespace UseCases.Users.Commands.Delete
{
    public class DeleteUsersHandler(IRepository<User> repository) : ICommandHandler<DeleteUsersCommand, Result>
    {
        private readonly IRepository<User> _repository = repository;

        public async ValueTask<Result> Handle(DeleteUsersCommand command, CancellationToken cancellationToken)
        {
            var users = await _repository.ListAsync(new UsersByIdsSpec(command.UserIds), cancellationToken);
            await _repository.DeleteRangeAsync(users, cancellationToken);
            return Result.Success();
        }
    }
}
