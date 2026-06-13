namespace Core.UserAggregate.ValueObjects
{
    [ValueObject<string>(conversions: Conversions.SystemTextJson)]
    public partial struct UserEmail
    {
        public const int MaxLength = 320;

        private static string NormalizeInput(string value) => value.Trim();

        private static Validation Validate(in string value) =>
            string.IsNullOrWhiteSpace(value)
                ? Validation.Invalid("Email cannot be empty.")
                : value.Length > MaxLength
                    ? Validation.Invalid($"Email cannot be longer than {MaxLength} characters.")
                    : !value.Contains('@')
                        ? Validation.Invalid("Email is invalid.")
                        : Validation.Ok;
    }
}
