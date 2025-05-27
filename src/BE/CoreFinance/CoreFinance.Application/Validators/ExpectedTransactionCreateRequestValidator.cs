using FluentValidation;
using CoreFinance.Application.DTOs.ExpectedTransaction;

namespace CoreFinance.Application.Validators;

public class ExpectedTransactionCreateRequestValidator : AbstractValidator<ExpectedTransactionCreateRequest>
{
    public ExpectedTransactionCreateRequestValidator()
    {
        RuleFor(x => x.RecurringTransactionTemplateId).NotEmpty();
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.ExpectedDate).NotEmpty();
        RuleFor(x => x.ExpectedAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TransactionType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
    }
} 