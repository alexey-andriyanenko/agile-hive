using FluentValidation;
using IdentityService.Contracts;

namespace IdentityService.Application.Validations;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address");
        
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(200)
            .WithMessage("First name cannot exceed 200 characters");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(200)
            .WithMessage("Last name cannot exceed 200 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")
            .WithMessage("Password must contain at least 8 characters, include 1 special character, 1 digit, 1 lowercase and 1 uppercase character.");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required")
            .MaximumLength(100)
            .WithMessage("Username cannot exceed 100 characters");
        
        RuleFor(x => x.OrganizationName)
            .MaximumLength(200)
            .WithMessage("Organization name cannot exceed 200 characters");
    }
}