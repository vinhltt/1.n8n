using CoreFinance.Domain.BaseRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Reflection;
using CoreFinance.Domain.UnitOffWorks;
using System.Runtime.CompilerServices;
using CoreFinance.Contracts.Attributes;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.Constants;
using CoreFinance.Contracts.Utilities;

namespace CoreFinance.Infrastructure.UnitOffWorks;

public class UnitOffWork<TContext>(
    TContext context,
    IServiceProvider serviceProvider
)
    : IUnitOffWork
    where TContext : DbContext
{
    private bool _disposed;
    private Dictionary<Type, object?>? _repositories;
    //private UserManager<User>? _userManager;

    public Task<int> SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        return context.SaveChanges();
    }

    public async Task DoWorkWithTransaction(Action action)
    {
        await using var trans = await context.Database.BeginTransactionAsync();
        try
        {
            action.Invoke();
            await trans.CommitAsync();
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task DoWorkWithTransaction(Task<Action> action)
    {
        await using var trans = await context.Database.BeginTransactionAsync();
        try
        {
            (await action).Invoke();
            await trans.CommitAsync();
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task<T> DoWorkWithTransaction<T>(Func<Task<T>> action)
    {
        await using var trans = await context.Database.BeginTransactionAsync();
        try
        {
            var result = await action.Invoke();
            await trans.CommitAsync();
            return result;
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task<string> GetTemplateQueryAsync(
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string path = ""
    )
    {
        var queryFilePath = BuildPathDao(path,
            methodName);

        return await File.ReadAllTextAsync(queryFilePath);
    }

    public IBaseRepository<TEntity, TKey> Repository<TEntity, TKey>()
        where TEntity : BaseEntity<TKey>
    {
        _repositories ??= new Dictionary<Type, object?>();

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
            _repositories[type] = serviceProvider.GetService<IBaseRepository<TEntity, TKey>>();
        return _repositories[type] as IBaseRepository<TEntity, TKey> ??
               throw new InvalidOperationException();
    }

    ~UnitOffWork()
    {
        Dispose(false);
    }

    /// <summary>
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(
        bool disposing
    )
    {
        if (!_disposed)
            if (disposing)
                context.Dispose();

        _disposed = true;
    }

    protected string BuildPathDao(
        string directory,
        string methodName
    )
    {
        if (methodName.EndsWith(CommonConst.ASYNC))
            methodName = methodName[..^CommonConst.ASYNC.Length];

        var queryFilePath = Path.Combine(FileHelper.GetApplicationFolder(),
            EnvironmentConst.RESOURCES_FOLDER,
            Path.GetFileNameWithoutExtension(directory),
            $"{methodName}.sql");

        if (!File.Exists(queryFilePath))
            throw new FileNotFoundException(queryFilePath);

        return queryFilePath;
    }

    public object[] BuildEfParameters<T>(
        T input
    )
        where T : class
    {
        var result = typeof(T).GetProperties()
                              .Select(prop => GetSqlParameterAttributeValue(input, prop))
                              .Where(param => param != null)
                              .Select(param => param!)
                              .ToArray();

        return result;
    }

    public virtual object? GetSqlParameterAttributeValue<T>(
        T input,
        PropertyInfo prop
    )
    {
        var type = input!.GetType()
                         .GetProperty(prop.Name);

        var attribute = type!.GetCustomAttributes(typeof(SqlParameterAttribute),
                                 true)
                             .FirstOrDefault();

        if (attribute == null)
            return null;

        var description = (SqlParameterAttribute)attribute;

        var value = prop.GetValue(input);

        var result = new NpgsqlParameter
        {
            DbType = description.DbType,
            ParameterName = description.ParameterName,
            Value = value
        };

        return result;
    }
}