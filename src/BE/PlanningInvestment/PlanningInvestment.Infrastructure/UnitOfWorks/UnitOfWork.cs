using PlanningInvestment.Domain.UnitOfWorks;
using PlanningInvestment.Infrastructure.Data;
using PlanningInvestment.Infrastructure.Repositories;
using PlanningInvestment.Domain.BaseRepositories;
using Shared.Contracts.BaseEfModels;
using Microsoft.EntityFrameworkCore.Storage;
using System.Runtime.CompilerServices;

namespace PlanningInvestment.Infrastructure.UnitOfWorks;

/// <summary>
/// Unit of work implementation for PlanningInvestment (EN)<br/>
/// Triển khai unit of work cho PlanningInvestment (VI)
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly PlanningInvestmentDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed = false;

    public UnitOfWork(PlanningInvestmentDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Save all changes to database (EN)<br/>
    /// Lưu tất cả thay đổi vào cơ sở dữ liệu (VI)
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Begin database transaction (EN)<br/>
    /// Bắt đầu transaction cơ sở dữ liệu (VI)
    /// </summary>
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Get template query for SQL operations (EN)<br/>
    /// Lấy template query cho các thao tác SQL (VI)
    /// </summary>
    public Task<string> GetTemplateQueryAsync(
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string path = ""
    )
    {
        // Simple implementation for template query
        var template = $"-- Generated query for {methodName} in {Path.GetFileName(path)}";
        return Task.FromResult(template);
    }

    /// <summary>
    /// Get repository for specified entity type (EN)<br/>
    /// Lấy repository cho loại entity được chỉ định (VI)
    /// </summary>
    public IBaseRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey> where TKey : struct
    {
        var type = typeof(TEntity);
        
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new BaseRepository<TEntity, TKey>(_context);
        }
        
        return (IBaseRepository<TEntity, TKey>)_repositories[type];
    }

    /// <summary>
    /// Commit database transaction (EN)<br/>
    /// Commit transaction cơ sở dữ liệu (VI)
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.CommitTransactionAsync();
        }
    }

    /// <summary>
    /// Rollback database transaction (EN)<br/>
    /// Rollback transaction cơ sở dữ liệu (VI)
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }

    /// <summary>
    /// Dispose resources (EN)<br/>
    /// Giải phóng tài nguyên (VI)
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
}
