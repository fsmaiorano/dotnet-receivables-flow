using FluentValidation;

namespace Application.UseCases.Account.Commands.UpdateAccount;

public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.PasswordHash)
            .NotEmpty().WithMessage("PasswordHash is required.")
            .MaximumLength(100).WithMessage("PasswordHash must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.");
    }
}