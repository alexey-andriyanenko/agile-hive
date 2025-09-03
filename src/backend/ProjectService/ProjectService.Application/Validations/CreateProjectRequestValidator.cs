using FluentValidation;
using ProjectService.Contracts;

namespace ProjectService.Application.Validations;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
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
        
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("OrganizationId is required")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("OrganizationId must be a valid GUID");
    }
}