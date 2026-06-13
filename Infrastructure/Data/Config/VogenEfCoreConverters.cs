namespace Infrastructure.Data.Config
{
    [EfCoreConverter<UserId>]
    [EfCoreConverter<UserName>]
    [EfCoreConverter<UserEmail>]
    [EfCoreConverter<NormalizedEmail>]
    [EfCoreConverter<PasswordHash>]
    [EfCoreConverter<EmailConfirmationToken>]
    internal partial class VogenEfCoreConverters
    {
    }
}
