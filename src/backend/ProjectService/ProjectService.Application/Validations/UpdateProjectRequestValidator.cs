using FluentValidation;
using ProjectService.Contracts;

namespace ProjectService.Application.Validations;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .MinimumLength(2)
            .WithMessage("Project name must be at least 2 characters long")
            .Matches(@"^[A-Za-z0-9 ]+$")
            .WithMessage("Project name can only contain letters, numbers, and spaces")
            .Matches(@"^(?!.* {2,}).*$")
            .WithMessage("Project name cannot contain consecutive spaces");
        
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot contain more than 500 characters");
    }
}