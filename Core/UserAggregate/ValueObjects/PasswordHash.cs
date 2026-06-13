namespace Core.UserAggregate.ValueObjects
{
    [ValueObject<string>]
    public partial struct PasswordHash
    {
        private static Validation Validate(in string value) =>
            string.IsNullOrWhiteSpace(value)
                ? Validation.Invalid("Password hash cannot be empty.")
                : Validation.Ok;
    }
}
