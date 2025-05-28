using Castle.DynamicProxy;
using CoreFinance.Api.Infrastructures.Interceptors;
using CoreFinance.Application.Interfaces;
using CoreFinance.Application.Services;
using CoreFinance.Contracts.Extensions;
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

        services.AddScoped<IUnitOfWork, UnitOfWork<CoreFinanceDbContext>>();

        services.AddProxiedScoped<IAccountService, AccountService>();
        services.AddProxiedScoped<ITransactionService, TransactionService>();
        services.AddProxiedScoped<IRecurringTransactionTemplateService, RecurringTransactionTemplateService>();
        services.AddProxiedScoped<IExpectedTransactionService, ExpectedTransactionService>();
    }
}