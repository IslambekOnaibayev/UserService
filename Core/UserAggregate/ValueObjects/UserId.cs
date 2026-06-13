namespace Core.UserAggregate.ValueObjects
{
    [ValueObject<Guid>]
    public readonly partial struct UserId
    {
        public static UserId New() => From(Guid.NewGuid());

        private static Validation Validate(Guid value) =>
            value == Guid.Empty
                ? Validation.Invalid("UserId cannot be empty.")
                : Validation.Ok;
    }
}
