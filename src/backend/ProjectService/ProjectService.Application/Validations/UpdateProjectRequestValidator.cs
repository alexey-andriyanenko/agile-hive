using FluentValidation;
using ProjectService.gRPC;

namespace ProjectService.Application.Validations;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot contain more than 200 characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot contain more than 500 characters");
        
        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Slug is required")
            .MaximumLength(200)
            .WithMessage("Slug cannot contain more than 200 characters");
    }
}