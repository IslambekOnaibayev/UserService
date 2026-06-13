namespace UseCases.Users.Queries.List
{
    public record ListUsersQuery : Mediator.IQuery<Result<IReadOnlyList<UserDto>>>;
}
