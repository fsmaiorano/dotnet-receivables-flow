using FluentValidation;

namespace Application.UseCases.Assignor.Commands.DeleteAssignor;

public class DeleteAssignorValidator : AbstractValidator<DeleteAssignorCommand>
{
    public DeleteAssignorValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }
}