namespace Core.UserAggregate.ValueObjects
{
    [ValueObject<string>]
    public partial struct EmailConfirmationToken
    {
        public const int MaxLength = 128;

        public static EmailConfirmationToken New()
        {
            var raw = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_", StringComparison.Ordinal)
                .Replace("+", "-", StringComparison.Ordinal)
                .TrimEnd('=');
            return From(raw);
        }

        private static Validation Validate(in string value) =>
            string.IsNullOrWhiteSpace(value)
                ? Validation.Invalid("Token cannot be empty.")
                : value.Length > MaxLength
                    ? Validation.Invalid($"Token cannot be longer than {MaxLength} characters.")
                    : Validation.Ok;
    }
}
