using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Domain.BaseRepositories;
using Microsoft.AspNetCore.Http;

namespace CoreFinance.Infrastructure.Repositories.Base;

/// <summary>
/// Base class for repositories.
/// </summary>
public class BaseRepository<TEntity, TKey>(
    CoreFinanceDbContext context,
    IHttpContextAccessor? httpContextAccessor
)
    : IBaseRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    #region props

    private DbSet<TEntity>? EntitiesDbSet { get; set; }

    #endregion props

    #region ctor

    public BaseRepository(CoreFinanceDbContext context) : this(context, null)
    {
    }

    // ReSharper disable once EmptyDestructor
    ~BaseRepository()
    {
    }

    #endregion ctor

    #region public

    public IQueryable<TEntity> GetNoTrackingEntities(
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Entities.AsNoTracking();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        query = query.Where(e => e.CreateBy == GetUserNameInHttpContext());
        return query;
    }

    public IQueryable<TEntity> GetNoTrackingEntitiesIdentityResolution(
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Entities.AsNoTrackingWithIdentityResolution();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        query = query.Where(e => e.CreateBy == GetUserNameInHttpContext());
        return query;
    }

    public IQueryable<TEntity> GetQueryableTable(
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Entities.AsQueryable();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        query = query.Where(e => e.CreateBy == GetUserNameInHttpContext());
        return query;
    }

    public virtual Task<List<TEntity>> GetAllAsync(
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = GetNoTrackingEntities();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var entities = query.ToListAsync();
        return entities;
    }

    public virtual Task<TEntity?> GetByIdAsync(TKey id,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Entities.AsQueryable();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        query = query.Where(e => e.CreateBy == GetUserNameInHttpContext());
        var entity = query.SingleOrDefaultAsync(x => x.Id!.Equals(id));
        return entity;
    }

    public virtual Task<TEntity?> GetByIdNoTrackingAsync(TKey id,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = GetNoTrackingEntities();
        foreach (var include in includes) query = query.Include(include);
        var entity = query.SingleOrDefaultAsync(x => x.Id!.Equals(id));
        return entity;
    }

    public virtual async Task<int> CreateAsync(TEntity entity)
    {
        ValidateAndThrow(entity);
        var currentUserName = GetUserNameInHttpContext();
        entity.SetDefaultValue(currentUserName);
        await Entities.AddAsync(entity);
        var countAffect = await context.SaveChangesAsync();
        return countAffect;
    }

    public virtual async Task<int> CreateAsync(List<TEntity> entities)
    {
        ValidateAndThrow(entities);
        var currentUserName = GetUserNameInHttpContext();
        entities.ForEach(e => { e.SetDefaultValue(currentUserName); });

        await Entities.AddRangeAsync(entities);
        var countAffect = await context.SaveChangesAsync();
        return countAffect;
    }

    public virtual Task<int> UpdateAsync(TEntity entity)
    {
        ValidateAndThrow(entity);
        var currentUserName = GetUserNameInHttpContext();
        var entry = context.Entry(entity);
        entity.SetValueUpdate(currentUserName);
        if (entry.State < EntityState.Added) entry.State = EntityState.Modified;
        var countAffect = context.SaveChanges();
        return Task.FromResult(countAffect);
    }

    public virtual Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        var currentUserName = GetUserNameInHttpContext();
        var baseEntities = entities.ToList();
        baseEntities.ForEach(e =>
        {
            ValidateAndThrow(e);
            e.SetValueUpdate(currentUserName);
        });

        var entry = context.Entry(baseEntities);
        if (entry.State < EntityState.Added) entry.State = EntityState.Modified;
        var countAffect = context.SaveChanges();
        return Task.FromResult(countAffect);
    }

    public virtual async Task<int> DeleteHardAsync(params object[] keyValues)
    {
        // Logic phức tạp hơn bên trong DeleteHardAsync để xây dựng biểu thức Where động
        // dựa trên keyValues và metadata của khóa chính
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        var primaryKey = entityType?.FindPrimaryKey();

        if (primaryKey == null || primaryKey.Properties.Count != keyValues.Length)
        {
            throw new ArgumentException("Number of key values does not match the number of primary key properties.");
        }

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? predicateBody = null;

        for (var i = 0; i < primaryKey.Properties.Count; i++)
        {
            var property = primaryKey.Properties[i];
            if (property.PropertyInfo == null) continue;
            var propertyAccess = Expression.MakeMemberAccess(parameter, property.PropertyInfo);
            var keyValue = Expression.Constant(keyValues[i], property.ClrType);
            var equality = Expression.Equal(propertyAccess, keyValue);

            predicateBody = predicateBody == null ? equality : Expression.AndAlso(predicateBody, equality);
        }

        var predicate = Expression.Lambda<Func<TEntity, bool>>(predicateBody!, parameter);

        return await Entities.Where(predicate).ExecuteDeleteAsync();

    }

    public virtual void DeleteHard(TEntity entity)
    {
        ValidateAndThrow(entity);
        Entities.Remove(entity);
    }

    public virtual async Task<int> DeleteSoftAsync(params object[] keyValues)
    {
        var entity = await context.Set<TEntity>().FindAsync(keyValues);
        ValidateAndThrow(entity);
        entity!.Deleted = DateTime.Now.ToString("yyyyMMddHHmmss");
        return await UpdateAsync(entity);
    }

    public virtual async Task<int> DeleteSoftAsync(TEntity entity)
    {
        ValidateAndThrow(entity);
        entity.Deleted = DateTime.Now.ToString("yyyyMMddHHmmss");
        return await UpdateAsync(entity);
    }

    #endregion public

    #region private

    protected DbSet<TEntity> Entities => EntitiesDbSet ??= context.Set<TEntity>();

    protected string GetUserNameInHttpContext()
    {
        var userName = httpContextAccessor?.HttpContext?.User
            .FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value ?? "SystemDefault";
        if (userName == null)
            throw new ArgumentNullException(nameof(userName));
        return userName;
    }

    protected void ValidateAndThrow(TEntity? entity)
    {
        if (entity != null) return;
        throw new ArgumentNullException(nameof(entity));
    }

    protected void ValidateAndThrow(IEnumerable<TEntity> entities)
    {
        if (entities == null || !entities.Any()) throw new ArgumentNullException(nameof(entities));
    }

    #endregion private
}