using FluentValidation;

namespace Application.UseCases.Payable.Commands.CreatePayable;

public class CreatePayableValidator : AbstractValidator<CreatePayableCommand>
{
    public CreatePayableValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required.")
            .GreaterThan(0).WithMessage("Value must be greater than 0.");

        RuleFor(x => x.EmissionDate)
            .NotEmpty().WithMessage("Emission date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Emission date must be less than or equal to the current date.");

        RuleFor(x => x.AssignorId)
            .NotEmpty().WithMessage("Assignor id is required.");

        RuleFor(x => x.Assignor)
            .NotEmpty().WithMessage("Assignor is required.");
    }
}