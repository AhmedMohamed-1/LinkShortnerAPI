using FluentValidation;
using LinkShorterAPI.DTOs.UserDTO;

namespace LinkShorterAPI.ClassValidators.UserValidatorClasses
{
    public class SignInDTOValidator : AbstractValidator<SignInDTO>
    {
        public SignInDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
} 