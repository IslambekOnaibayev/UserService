namespace Core.UserAggregate.ValueObjects
{
    [ValueObject<string>(conversions: Conversions.SystemTextJson)]
    public partial struct UserName
    {
        public const int MaxLength = 100;

        private static Validation Validate(in string value) =>
            string.IsNullOrWhiteSpace(value)
                ? Validation.Invalid("Name cannot be empty.")
                : value.Length > MaxLength
                    ? Validation.Invalid($"Name cannot be longer than {MaxLength} characters.")
                    : Validation.Ok;
    }
}
