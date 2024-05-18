using FluentValidation;

namespace Application.UseCases.Assignor.Commands.UpdateAssignor;

public class UpdateAssignorValidator : AbstractValidator<UpdateAssignorCommand>
{
    public UpdateAssignorValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Document is required.")
            .MaximumLength(14).WithMessage("Document must not exceed 14 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.");
    }
}