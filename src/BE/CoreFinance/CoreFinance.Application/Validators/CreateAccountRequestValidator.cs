using FluentValidation;
using CoreFinance.Application.DTOs;

namespace CoreFinance.Application.Validators;

public class CreateAccountRequestValidator : AbstractValidator<AccountCreateRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AccountName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AccountType).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.InitialBalance).GreaterThanOrEqualTo(0);
    }
}