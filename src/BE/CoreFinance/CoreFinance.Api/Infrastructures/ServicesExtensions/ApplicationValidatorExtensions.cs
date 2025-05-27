using CoreFinance.Application.Validators;
using FluentValidation;

namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public static class ApplicationValidatorExtensions
{
    public static void AddApplicationValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateAccountRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateTransactionRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateTransactionRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<RecurringTransactionTemplateCreateRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<RecurringTransactionTemplateUpdateRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<ExpectedTransactionCreateRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<ExpectedTransactionUpdateRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<ConfirmTransactionRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<AdjustTransactionRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<CancelTransactionRequestValidator>();
    }
}