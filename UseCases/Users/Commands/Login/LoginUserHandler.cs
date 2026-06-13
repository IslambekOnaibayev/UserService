namespace UseCases.Users.Commands.Login
{
    public class LoginUserHandler(IRepository<User> repository, IPasswordHasher passwordHasher)
        : ICommandHandler<LoginUserCommand, Result<Guid>>
    {
        private readonly IRepository<User> _repository = repository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async ValueTask<Result<Guid>> Handle(
            LoginUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _repository.FirstOrDefaultAsync(
                new UserByEmailSpec(command.Email), cancellationToken);

            if (user is null)
                return Result.Unauthorized();

            if (user.IsBlocked)
                return Result.Forbidden();

            if (!_passwordHasher.Verify(user.PasswordHash.Value, command.Password))
                return Result.Unauthorized();

            user.MarkLoggedIn(DateTimeOffset.UtcNow);
            await _repository.UpdateAsync(user, cancellationToken);

            return Result.Success(user.Id.Value);
        }
    }
}
