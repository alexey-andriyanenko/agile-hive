using FluentValidation;
using OrganizationService.Contracts;

namespace OrganizationService.Application.Validators;

public class UpdateOrganizationRequestValidator : AbstractValidator<UpdateOrganizationRequest>
{
    public UpdateOrganizationRequestValidator()
    {
        RuleFor(x => x.OrganizationName)
            .NotEmpty()
            .WithMessage("Organization name is required")
            .MinimumLength(2)
            .WithMessage("Organization name must be at least 2 characters long")
            .Matches(@"^[A-Za-z0-9 ]+$")
            .WithMessage("Organization name can only contain letters, numbers, and spaces")
            .Matches(@"^(?!.* {2,}).*$")
            .WithMessage("Organization name cannot contain consecutive spaces");
    }
}