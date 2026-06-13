using FluentValidation;

namespace WebAPI.Endpoints.Users
{
    public class RegisterValidator : Validator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(UserName.MaxLength);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(UserEmail.MaxLength);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(1);
        }
    }
}
