using Castle.DynamicProxy;
using CoreFinance.Api.Infrastructures.Interceptors;
using CoreFinance.Application.Interfaces;
using CoreFinance.Application.Services;
using CoreFinance.Application.Validators;
using CoreFinance.Contracts.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using CoreFinance.Domain.UnitOfWorks;
using CoreFinance.Infrastructure;
using CoreFinance.Infrastructure.UnitOfWorks;

namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IProxyGenerator, ProxyGenerator>();
        services.AddScoped<IAsyncInterceptor, MonitoringInterceptor>();
        services.AddProxiedScoped<IAccountService, AccountService>();
        services.AddProxiedScoped<ITransactionService, TransactionService>();

        services.AddScoped<IUnitOfWork, UnitOfWork<CoreFinanceDbContext>>();

        // Register new services
        services.AddProxiedScoped<IRecurringTransactionTemplateService, RecurringTransactionTemplateService>();
        services.AddProxiedScoped<IExpectedTransactionService, ExpectedTransactionService>();

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
    }
}