namespace Core.UserAggregate.ValueObjects
{
    [ValueObject<string>]
    public partial struct NormalizedEmail
    {
        public static NormalizedEmail FromEmail(UserEmail email) =>
            From(email.Value.Trim().ToUpperInvariant());

        private static string NormalizeInput(string value) =>
            value.Trim().ToUpperInvariant();

        private static Validation Validate(in string value) =>
            string.IsNullOrWhiteSpace(value)
                ? Validation.Invalid("Normalized email cannot be empty.")
                : Validation.Ok;
    }
}
