using FluentValidation;
using TaskService.Contracts;

namespace TaskService.Application.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");
        
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("BoardId is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("BoardId must be a valid GUID.");

        RuleFor(x => x.BoardColumnId)
            .NotEmpty().WithMessage("BoardColumnId is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("BoardColumnId must be a valid GUID.");
        
        RuleFor(x => x.AssigneeUserId)
            .Must(id => string.IsNullOrEmpty(id) || Guid.TryParse(id, out _))
            .WithMessage("AssigneeUserId must be a valid GUID if provided.");
    }
}