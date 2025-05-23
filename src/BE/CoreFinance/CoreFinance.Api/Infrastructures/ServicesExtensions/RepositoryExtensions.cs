using CoreFinance.Contracts.Extensions;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Infrastructure.Repositories.Base;

namespace CoreFinance.Api.Infrastructures.ServicesExtensions;

public static class RepositoryExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddProxiedScoped<IBaseRepository<Account, Guid>, BaseRepository<Account, Guid>>();
        services.AddProxiedScoped<IBaseRepository<Transaction, Guid>, BaseRepository<Transaction, Guid>>();
    }
}