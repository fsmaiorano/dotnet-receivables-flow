namespace Application.UseCases.Payable.Commands.ProcessPayableBatch;

using FluentValidation;

public class ProcessPayablesBatchValidator : AbstractValidator<ProcessPayablesBatchCommand>
{
    public ProcessPayablesBatchValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Payables)
            .NotEmpty().WithMessage("Payables are required.")
            .Must(x => x.Count > 0).WithMessage("Payables must have at least one item.");
    }
}
