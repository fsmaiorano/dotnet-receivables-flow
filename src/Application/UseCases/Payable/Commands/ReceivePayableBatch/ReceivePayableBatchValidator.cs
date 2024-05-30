namespace Application.UseCases.Payable.Commands.ReceivePayableBatch;

using Application.UseCases.Payable.Commands.CreatePayableReceiveBatch;
using FluentValidation;

public class ReceivePayableBatchValidator : AbstractValidator<ReceivePayableBatchCommand>
{
    public ReceivePayableBatchValidator()
    {
        RuleFor(x => x.Payables)
            .NotEmpty().WithMessage("Payables are required.")
            .Must(x => x.Count > 0).WithMessage("Payables must have at least one item.");
    }
}
