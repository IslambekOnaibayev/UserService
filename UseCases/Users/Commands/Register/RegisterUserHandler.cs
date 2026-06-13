namespace UseCases.Users.Commands.Register
{
    public class RegisterUserHandler(IRepository<User> repository, IPasswordHasher passwordHasher)
        : ICommandHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IRepository<User> _repository = repository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async ValueTask<Result<Guid>> Handle(
            RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var existing = await _repository.FirstOrDefaultAsync(
                new UserByEmailSpec(command.Email), cancellationToken);

            if (existing is not null)
                return Result.Conflict("A user with this email already exists.");

            var hash = _passwordHasher.Hash(command.Password);
            var user = User.Register(command.Name, command.Email, hash);

            await _repository.AddAsync(user, cancellationToken);

            return Result.Success(user.Id.Value);
        }
    }
}
