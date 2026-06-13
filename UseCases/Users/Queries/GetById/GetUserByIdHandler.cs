namespace UseCases.Users.Queries.GetById
{
    public class GetUserByIdHandler(IReadRepository<User> repository)
        : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IReadRepository<User> _repository = repository;

        public async ValueTask<Result<UserDto>> Handle(
            GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(UserId.From(query.UserId), cancellationToken);
            if (user is null) return Result.NotFound();

            return new UserDto(
                user.Id.Value,
                user.Name.Value,
                user.Email.Value,
                user.RegistrationTime,
                user.LastLoginTime,
                user.LastActivityTime,
                user.Status.Name);
        }
    }
}
