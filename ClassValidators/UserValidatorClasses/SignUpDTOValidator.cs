using FluentValidation;
using LinkShorterAPI.DTOs.UserDTO;

namespace LinkShorterAPI.ClassValidators.UserValidatorClasses
{
    public class SignUpDTOValidator : AbstractValidator<SignUpDTO>
    {
        public SignUpDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
        }
    }
} 