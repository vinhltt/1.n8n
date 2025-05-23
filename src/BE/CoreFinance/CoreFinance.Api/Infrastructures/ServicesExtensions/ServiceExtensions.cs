using Castle.DynamicProxy;
using CoreFinance.Api.Infrastructures.Interceptors;
using CoreFinance.Application.Interfaces;
using CoreFinance.Application.Services;
using CoreFinance.Contracts.Extensions;

namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IProxyGenerator, ProxyGenerator>();
        services.AddScoped<IAsyncInterceptor, MonitoringInterceptor>();
        services.AddProxiedScoped<IAccountService, AccountService>();

        services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateAccountRequestValidator>();

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
    }
}