using System.Linq.Expressions;
using CoreFinance.Contracts.BaseEfModels;

namespace CoreFinance.Domain.BaseRepositories;

/// <summary>
/// Base interface for repositories.
/// </summary>

public interface IBaseRepository<TEntity, in TKey> where TEntity : BaseEntity<TKey>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> CreateAsync(List<TEntity> entities);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> CreateAsync(TEntity entity);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(TEntity entity);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(IEnumerable<TEntity> entities);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyValues"></param>
    /// <returns></returns>
    Task<int> DeleteHardAsync(params object[] keyValues);
    /// <summary>s
    /// 
    /// </summary>
    /// <param name="entity"></param>
    void DeleteHard(TEntity entity);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyValues"></param>
    /// <returns></returns>
    Task<int> DeleteSoftAsync(params object[] keyValues);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> DeleteSoftAsync(TEntity entity);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TEntity?> GetByIdNoTrackingAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="includes"></param>
    /// <returns></returns>
    IQueryable<TEntity> GetQueryableTable(params Expression<Func<TEntity, object>>[] includes);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="includes"></param>
    /// <returns></returns>
    IQueryable<TEntity> GetNoTrackingEntities(params Expression<Func<TEntity, object>>[] includes);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="includes"></param>
    /// <returns></returns>
    IQueryable<TEntity> GetNoTrackingEntitiesIdentityResolution(params Expression<Func<TEntity, object>>[] includes);
}