using FluentValidation;
using LinkShorterAPI.DTOs.UserDTO;

namespace LinkShorterAPI.ClassValidators.UserValidatorClasses
{
    public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserDTOValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.NewPassword)
                .MinimumLength(6).When(x => !string.IsNullOrEmpty(x.NewPassword))
                .WithMessage("New password must be at least 6 characters long.");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.NewPassword))
                .WithMessage("Current password is required when changing password.");
        }
    }
} 