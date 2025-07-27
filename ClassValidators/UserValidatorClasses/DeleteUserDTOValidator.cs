using FluentValidation;
using LinkShorterAPI.DTOs.UserDTO;

namespace LinkShorterAPI.ClassValidators.UserValidatorClasses
{
    public class DeleteUserDTOValidator : AbstractValidator<DeleteUserDTO>
    {
        public DeleteUserDTOValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*?&]).{8,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one special character, and be at least 8 characters long.");
        }
    }
}
