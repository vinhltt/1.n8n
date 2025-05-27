using FluentValidation;
using CoreFinance.Application.DTOs.RecurringTransactionTemplate;

namespace CoreFinance.Application.Validators;

public class RecurringTransactionTemplateCreateRequestValidator : AbstractValidator<RecurringTransactionTemplateCreateRequest>
{
    public RecurringTransactionTemplateCreateRequestValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.Frequency).IsInEnum();
        RuleFor(x => x.TransactionType).IsInEnum();
    }
} 