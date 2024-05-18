using FluentValidation;

namespace Application.UseCases.Payable.Commands.UpdatePayable;

public class UpdatePayableValidator : AbstractValidator<UpdatePayableCommand>
{
    public UpdatePayableValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required.");

        RuleFor(x => x.EmissionDate)
            .NotEmpty().WithMessage("EmissionDate is required.");

        RuleFor(x => x.AssignorId)
            .NotEmpty().WithMessage("AssignorId is required.");
    }
}