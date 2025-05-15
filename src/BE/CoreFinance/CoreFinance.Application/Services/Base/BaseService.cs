using AutoMapper;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Contracts.Utilities;
using CoreFinance.Domain.UnitOffWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreFinance.Application.Services.Base;

/// <summary>
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TCreateRequest"></typeparam>
/// <typeparam name="TUpdateRequest"></typeparam>
/// <typeparam name="TViewModel"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class BaseService<TEntity, TCreateRequest, TUpdateRequest, TViewModel,
    TKey>(
    IMapper mapper,
    IUnitOffWork unitOffWork,
    ILogger logger
)
    : IBaseService< TEntity, TCreateRequest, TUpdateRequest, TViewModel, TKey>
    where TEntity : BaseEntity<TKey>, new()
    where TCreateRequest : BaseCreateRequest, new()
    where TUpdateRequest : BaseUpdateRequest<TKey>, new()
    where TViewModel : BaseViewModel<TKey>, new()
{
    protected readonly IMapper Mapper = mapper;
    protected readonly IUnitOffWork UnitOffWork = unitOffWork;

    /// <summary>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<int?> DeleteHardAsync(TKey id)
    {
        logger.LogTrace($"DeleteHardAsync: {id.TryParseToString()}");
        await UnitOffWork.Repository<TEntity, TKey>().DeleteHardAsync(id!);
        return await UnitOffWork.SaveChangesAsync();
    }

    /// <summary>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<int?> DeleteSoftAsync(TKey id)
    {
        logger.LogTrace($"DeleteSoftAsync: {id.TryParseToString()}");
        return await UnitOffWork.Repository<TEntity, TKey>().DeleteSoftAsync(id!);
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TViewModel>?> GetAllDtoAsync()
    {
        var query = UnitOffWork.Repository<TEntity, TKey>();
        var result = await Mapper.ProjectTo<TViewModel>(query.GetNoTrackingEntities())
            .ToListAsync();
        logger.LogTrace($"GetAllDtoAsync result: {result.TryParseToString()}");
        return result;
    }

    public virtual async Task<TViewModel?> GetByIdAsync(TKey id)
    {
        logger.LogTrace($"GetByIdAsync: {id.TryParseToString()}");
        var entity = await UnitOffWork.Repository<TEntity, TKey>().GetByIdNoTrackingAsync(id);
        var result = Mapper.Map<TViewModel>(entity);
        logger.LogTrace($"GetByIdAsync result: {result.TryParseToString()}");
        return result;
    }

    public virtual async Task<TViewModel?> UpdateAsync(TKey id, TUpdateRequest request)
    {
        logger.LogTrace($"UpdateAsync request: {id}, {request.TryParseToString()}");
        if (id is null || !id.Equals(request.Id))
            throw new KeyNotFoundException();
        var entity = await UnitOffWork.Repository<TEntity, TKey>().GetByIdAsync(id);
        logger.LogTrace($"UpdateAsync old entity: {entity.TryParseToString()}");

        if (entity == null) throw new NullReferenceException();
        entity = Mapper.Map(request, entity);
        logger.LogTrace($"UpdateAsync new entity: {entity.TryParseToString()}");
        var effectedCount = await UnitOffWork.Repository<TEntity, TKey>().UpdateAsync(entity);
        logger.LogTrace($"UpdateAsync effectedCount: {effectedCount}");
        if (effectedCount <= 0) throw new NullReferenceException();
        var result = Mapper.Map<TViewModel>(entity);
        logger.LogTrace($"UpdateAsync result: {result.TryParseToString()}");
        return result;
    }

    /// <summary>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public virtual async Task<TViewModel?> CreateAsync(TCreateRequest request)
    {
        return await UnitOffWork.DoWorkWithTransaction(async () =>
        {
            logger.LogTrace($"CreateAsync request: {request.TryParseToString()}");
            var entityNew = new TEntity();
            Mapper.Map(request, entityNew);
            logger.LogTrace($"CreateAsync entitiesNew: {entityNew.TryParseToString()}");
            var effectedCount =
                await UnitOffWork.Repository<TEntity, TKey>().CreateAsync(entityNew);
            logger.LogTrace($"CreateAsync affectedCount: {effectedCount}");
            if (effectedCount <= 0) throw new NullReferenceException();
            var result = Mapper.Map<TViewModel>(entityNew);
            logger.LogTrace($"CreateAsync result: {result.TryParseToString()}");
            return result;
        });
    }

    /// <summary>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public virtual async Task<IEnumerable<TViewModel>?> CreateAsync(
        List<TCreateRequest> request)
    {
        if (!request.Any())
        {
            logger.LogInformation("Request Is Empty");
            return new List<TViewModel>();
        }

        return await UnitOffWork.DoWorkWithTransaction(async () =>
        {
            var baseCreateRequests = request as TCreateRequest[] ?? request.ToArray();
            logger.LogTrace($"CreateAsync request: {baseCreateRequests.TryParseToString()}");

            var entitiesNew = new List<TEntity>();
            Mapper.Map(baseCreateRequests, entitiesNew);
            logger.LogTrace($"CreateAsync entitiesNew: {entitiesNew.TryParseToString()}");

            var effectedCount =
                await UnitOffWork.Repository<TEntity, TKey>().CreateAsync(entitiesNew);
            logger.LogTrace($"CreateAsync affectedCount: {effectedCount}");

            if (effectedCount <= 0) throw new NullReferenceException();

            var result = Mapper.Map<IEnumerable<TViewModel>>(entitiesNew);
            var baseViewModels = result as TViewModel[] ?? result.ToArray();
            logger.LogTrace($"CreateAsync result: {baseViewModels.TryParseToString()}");
            return baseViewModels;
        });
    }
}