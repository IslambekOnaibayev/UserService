namespace UseCases.Users.Commands.ConfirmEmail
{
    public class ConfirmEmailHandler(IRepository<User> repository)
    : ICommandHandler<ConfirmEmailCommand, Result>
    {
        private readonly IRepository<User> _repository = repository;

        public async ValueTask<Result> Handle(
            ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(
                UserId.From(command.UserId), cancellationToken);

            if (user is null) return Result.NotFound();

            try
            {
                user.ConfirmEmail(command.Token);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Error(ex.Message);
            }

            await _repository.UpdateAsync(user, cancellationToken);
            return Result.Success();
        }
    }
}
