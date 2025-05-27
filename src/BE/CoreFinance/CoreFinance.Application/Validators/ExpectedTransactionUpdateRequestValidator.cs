using FluentValidation;
using CoreFinance.Application.DTOs.ExpectedTransaction;

namespace CoreFinance.Application.Validators;

public class ExpectedTransactionUpdateRequestValidator : AbstractValidator<ExpectedTransactionUpdateRequest>
{
    public ExpectedTransactionUpdateRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        // Add validation rules for other properties if needed
        // Example: RuleFor(x => x.ExpectedAmount).GreaterThanOrEqualTo(0).When(x => x.ExpectedAmount.HasValue);
        // Example: RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
    }
} 