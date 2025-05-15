using CoreFinance.Application.Mapper;
using CoreFinance.Domain.UnitOffWorks;
using CoreFinance.Infrastructure;
using CoreFinance.Infrastructure.UnitOffWorks;

namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public static class InjectionServiceExtension
{
    public static void AddInjectedServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddScoped(typeof(IUnitOffWork), typeof(UnitOffWork<CoreFinanceDbContext>));
        services.AddRepositories();
        services.AddServices();
    }
}