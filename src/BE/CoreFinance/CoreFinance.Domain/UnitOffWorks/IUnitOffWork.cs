using CoreFinance.Domain.BaseRepositories;
using System.Runtime.CompilerServices;
using CoreFinance.Contracts.BaseEfModels;

namespace CoreFinance.Domain.UnitOffWorks;

public interface IUnitOffWork : IDisposable
{
    Task<int> SaveChangesAsync();
    int SaveChanges();
    Task DoWorkWithTransaction(Action action);
    Task DoWorkWithTransaction(Task<Action> action);
    //Task<T> DoWorkWithTransaction<T>(Func<T> action);
    Task<T> DoWorkWithTransaction<T>(Func<Task<T>> action);

    Task<string> GetTemplateQueryAsync(
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string path = ""
    );
    IBaseRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
}