using FluentValidation;

namespace Application.UseCases.Payable.Commands.CreatePayableBatch;

public class CreatePayableBatchValidator : AbstractValidator<CreatePayableBatchCommand>
{
    public CreatePayableBatchValidator()
    {
        RuleFor(x => x.Payables)
            .NotEmpty().WithMessage("Payables are required.")
            .Must(x => x.Count > 0).WithMessage("Payables must have at least one item.");
    }
}