namespace UseCases.Users.Queries.GetById
{
    public record GetUserByIdQuery(Guid UserId) : Mediator.IQuery<Result<UserDto>>;
}
