using Castle.DynamicProxy;
using CoreFinance.Api.Infrastructures.Interceptors;
using CoreFinance.Application.Interfaces;
using CoreFinance.Application.Services;
using CoreFinance.Application.Validators;
using CoreFinance.Contracts.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IProxyGenerator, ProxyGenerator>();
        services.AddScoped<IAsyncInterceptor, MonitoringInterceptor>();
        services.AddProxiedScoped<IAccountService, AccountService>();
        services.AddProxiedScoped<ITransactionService, TransactionService>();

        services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateAccountRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateTransactionRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateTransactionRequestValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
    }
}