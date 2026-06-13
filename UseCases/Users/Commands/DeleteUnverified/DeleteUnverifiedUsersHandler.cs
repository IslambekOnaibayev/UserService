namespace UseCases.Users.Commands.DeleteUnverified
{
    public class DeleteUnverifiedUsersHandler(IRepository<User> repository)
        : ICommandHandler<DeleteUnverifiedUsersCommand, Result>
    {
        private readonly IRepository<User> _repository = repository;

        public async ValueTask<Result> Handle(
            DeleteUnverifiedUsersCommand command, CancellationToken cancellationToken)
        {
            var users = await _repository.ListAsync(new UnverifiedUsersSpec(), cancellationToken);
            await _repository.DeleteRangeAsync(users, cancellationToken);
            return Result.Success();
        }
    }

}
