using BoardService.Contracts;
using FluentValidation;

namespace BoardService.Application.Validators;

public class UpdateBoardColumnRequestValidator : AbstractValidator<UpdateBoardColumnRequest>
{
    public UpdateBoardColumnRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");
        
        RuleFor(x => x.BoardColumnId)
            .NotEmpty()
            .WithMessage("Id is required.");
        
        RuleFor(x => x.BoardId)
            .NotEmpty()
            .WithMessage("BoardId is required.");
    }
}