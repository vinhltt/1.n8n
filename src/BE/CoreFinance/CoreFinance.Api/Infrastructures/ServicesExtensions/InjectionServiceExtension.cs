using CoreFinance.Application.Mapper;
using CoreFinance.Domain.UnitOfWorks;
using CoreFinance.Infrastructure;
using CoreFinance.Infrastructure.UnitOfWorks;

namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public static class InjectionServiceExtension
{
    public static void AddInjectedServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddScoped<IUnitOfWork, UnitOfWork<CoreFinanceDbContext>>();
        services.AddApplicationValidators();
        services.AddRepositories();
        services.AddServices();
    }
}