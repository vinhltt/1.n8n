using FluentValidation;
using CoreFinance.Application.DTOs;

namespace CoreFinance.Application.Validators;

public class UpdateAccountRequestValidator : AbstractValidator<AccountUpdateRequest>
{
    public UpdateAccountRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
    }
}