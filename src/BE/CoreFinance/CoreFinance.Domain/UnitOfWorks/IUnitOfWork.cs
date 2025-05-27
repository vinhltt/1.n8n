using System.Runtime.CompilerServices;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Domain.BaseRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CoreFinance.Domain.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();

    Task<string> GetTemplateQueryAsync(
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string path = ""
    );

    IBaseRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
    Task<IDbContextTransaction> BeginTransactionAsync();
}