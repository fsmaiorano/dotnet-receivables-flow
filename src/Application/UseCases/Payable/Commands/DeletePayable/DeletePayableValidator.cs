using FluentValidation;

namespace Application.UseCases.Payable.Commands.DeletePayable;

public class PayableValidator : AbstractValidator<DeletePayableCommand>
{
    public PayableValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }
}