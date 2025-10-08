using FluentValidation;
using Tag.Contracts;

namespace TagService.Application.Validators;

public class UpdateTagRequestValidator : AbstractValidator<UpdateTagRequest>
{
    public UpdateTagRequestValidator()
    {
        RuleFor(x => x.TagId)
            .NotEmpty().WithMessage("Id is required.")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("Id must be a valid GUID.");
        
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required.")
            .Must(tid => Guid.TryParse(tid, out _))
            .WithMessage("TenantId must be a valid GUID.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Color)
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$").When(x => !string.IsNullOrEmpty(x.Color))
            .WithMessage("Color must be a valid hex color code.");
    }
}