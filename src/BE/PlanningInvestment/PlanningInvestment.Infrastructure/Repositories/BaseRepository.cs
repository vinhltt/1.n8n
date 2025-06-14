using PlanningInvestment.Domain.BaseRepositories;
using PlanningInvestment.Infrastructure.Data;
using Shared.Contracts.BaseEfModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PlanningInvestment.Infrastructure.Repositories;

/// <summary>
/// Base repository implementation for PlanningInvestment entities (EN)<br/>
/// Triển khai repository cơ sở cho các entity PlanningInvestment (VI)
/// </summary>
/// <typeparam name="TEntity">Entity type (EN) / Loại entity (VI)</typeparam>
/// <typeparam name="TKey">Primary key type (EN) / Loại khóa chính (VI)</typeparam>
public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : struct
{
    protected readonly PlanningInvestmentDbContext _context;    public BaseRepository(PlanningInvestmentDbContext context)
    {
        _context = context;
    }    public virtual async Task<int> CreateAsync(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.CreateAt = DateTime.UtcNow;
            entity.Deleted = "false";
        }
        await _context.Set<TEntity>().AddRangeAsync(entities);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> CreateAsync(TEntity entity)
    {
        entity.CreateAt = DateTime.UtcNow;
        entity.Deleted = "false";
        await _context.Set<TEntity>().AddAsync(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> UpdateAsync(TEntity entity)
    {
        entity.UpdateAt = DateTime.UtcNow;
        _context.Set<TEntity>().Update(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.UpdateAt = DateTime.UtcNow;
        }
        _context.Set<TEntity>().UpdateRange(entities);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> DeleteHardAsync(params object[] keyValues)
    {
        var entity = await _context.Set<TEntity>().FindAsync(keyValues);
        if (entity == null) return 0;
        
        _context.Set<TEntity>().Remove(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual void DeleteHard(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public virtual async Task<int> DeleteSoftAsync(params object[] keyValues)
    {
        var entity = await _context.Set<TEntity>().FindAsync(keyValues);
        if (entity == null) return 0;
        
        entity.Deleted = "true";
        entity.UpdateAt = DateTime.UtcNow;
        _context.Set<TEntity>().Update(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> DeleteSoftAsync(TEntity entity)
    {
        entity.Deleted = "true";
        entity.UpdateAt = DateTime.UtcNow;
        _context.Set<TEntity>().Update(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>().Where(e => e.Deleted != "true");
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>().Where(e => e.Id!.Equals(id) && e.Deleted != "true");
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<TEntity?> GetByIdNoTrackingAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>().AsNoTracking().Where(e => e.Id!.Equals(id) && e.Deleted != "true");
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync();
    }

    public virtual IQueryable<TEntity> GetQueryableTable(params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>().Where(e => e.Deleted != "true");
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return query;
    }

    public virtual IQueryable<TEntity> GetNoTrackingEntities(params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>().AsNoTracking().Where(e => e.Deleted != "true");
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return query;
    }

    public virtual IQueryable<TEntity> GetNoTrackingEntitiesIdentityResolution(params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>().AsNoTrackingWithIdentityResolution().Where(e => e.Deleted != "true");
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return query;
    }    // Legacy methods for backward compatibility
    public virtual async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await GetByIdAsync(id, Array.Empty<Expression<Func<TEntity, object>>>());
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await GetAllAsync(Array.Empty<Expression<Func<TEntity, object>>>());
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreateAt = DateTime.UtcNow;
        entity.Deleted = "false";
        var result = await _context.Set<TEntity>().AddAsync(entity);
        return result.Entity;
    }

    public virtual async Task<bool> DeleteAsync(TKey id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        // Soft delete
        entity.Deleted = "true";
        entity.UpdateAt = DateTime.UtcNow;
        _context.Set<TEntity>().Update(entity);
        return true;
    }

    public virtual async Task<bool> ExistsAsync(TKey id)
    {
        return await Task.FromResult(_context.Set<TEntity>().Any(e => e.Id!.Equals(id) && e.Deleted != "true"));
    }

    public virtual async Task<int> CountAsync()
    {
        return await Task.FromResult(_context.Set<TEntity>().Count(e => e.Deleted != "true"));
    }

    public virtual IQueryable<TEntity> GetQueryable()
    {
        return _context.Set<TEntity>().Where(e => e.Deleted != "true");
    }
}
