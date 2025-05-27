using FluentValidation;
using CoreFinance.Application.DTOs.RecurringTransactionTemplate;

namespace CoreFinance.Application.Validators;

public class RecurringTransactionTemplateUpdateRequestValidator : AbstractValidator<RecurringTransactionTemplateUpdateRequest>
{
    public RecurringTransactionTemplateUpdateRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().When(x => x.Name != null).MaximumLength(100);
        RuleFor(x => x.Amount).GreaterThan(0).When(x => x.Amount.HasValue);
        RuleFor(x => x.StartDate).NotEmpty().When(x => x.StartDate.HasValue);
        RuleFor(x => x.Frequency).IsInEnum().When(x => x.Frequency.HasValue);
        RuleFor(x => x.TransactionType).IsInEnum().When(x => x.TransactionType.HasValue);
    }
} 