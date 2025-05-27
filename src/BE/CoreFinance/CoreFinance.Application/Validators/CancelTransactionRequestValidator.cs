using FluentValidation;
using CoreFinance.Application.DTOs.ExpectedTransaction;

namespace CoreFinance.Application.Validators;

public class CancelTransactionRequestValidator : AbstractValidator<CancelTransactionRequest>
{
    public CancelTransactionRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().When(x => !string.IsNullOrEmpty(x.Reason)); // Validate if reason is provided
    }
} 