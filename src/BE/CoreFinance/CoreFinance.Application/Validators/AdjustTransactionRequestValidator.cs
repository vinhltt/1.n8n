using FluentValidation;
using CoreFinance.Application.DTOs.ExpectedTransaction;

namespace CoreFinance.Application.Validators;

public class AdjustTransactionRequestValidator : AbstractValidator<AdjustTransactionRequest>
{
    public AdjustTransactionRequestValidator()
    {
        RuleFor(x => x.NewAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Reason).NotEmpty().When(x => !string.IsNullOrEmpty(x.Reason)); // Validate if reason is provided
    }
} 