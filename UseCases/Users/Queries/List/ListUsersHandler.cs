namespace UseCases.Users.Queries.List
{
    public class ListUsersHandler(IReadRepository<User> repository)
        : IQueryHandler<ListUsersQuery, Result<IReadOnlyList<UserDto>>>
    {
        private readonly IReadRepository<User> _repository = repository;

        public async ValueTask<Result<IReadOnlyList<UserDto>>> Handle(
            ListUsersQuery query, CancellationToken cancellationToken)
        {
            var users = await _repository.ListAsync(new UserListSpec(), cancellationToken);

            var dtos = users
                .Select(u => new UserDto(
                    u.Id.Value,
                    u.Name.Value,
                    u.Email.Value,
                    u.RegistrationTime,
                    u.LastLoginTime,
                    u.LastActivityTime,
                    u.Status.Name))
                .ToList();

            return Result.Success<IReadOnlyList<UserDto>>(dtos);
        }
    }
}
