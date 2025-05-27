using FluentValidation;
using CoreFinance.Application.DTOs.ExpectedTransaction;

namespace CoreFinance.Application.Validators;

public class ConfirmTransactionRequestValidator : AbstractValidator<ConfirmTransactionRequest>
{
    public ConfirmTransactionRequestValidator()
    {
        RuleFor(x => x.ActualTransactionId).NotEmpty();
    }
} 