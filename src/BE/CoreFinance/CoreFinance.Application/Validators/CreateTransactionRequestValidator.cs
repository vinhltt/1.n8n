using CoreFinance.Application.DTOs;
using FluentValidation;

namespace CoreFinance.Application.Validators;

public class CreateTransactionRequestValidator : AbstractValidator<TransactionCreateRequest>
{
    public CreateTransactionRequestValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TransactionDate).NotEmpty();
        RuleFor(x => x.RevenueAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SpentAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryType).IsInEnum();
        RuleFor(x => x.Balance).GreaterThanOrEqualTo(0);
    }
}
