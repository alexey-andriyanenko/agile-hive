using BoardService.Contracts;
using FluentValidation;

namespace BoardService.Application.Validators;

public class CreateBoardColumnRequestValidator : AbstractValidator<CreateBoardColumnRequest>
{
    public CreateBoardColumnRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("BoardId is required.");
    }
}