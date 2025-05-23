using AutoMapper;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Contracts.Utilities;
using CoreFinance.Domain.Exceptions;
using CoreFinance.Domain.UnitOfWorks;
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
    IUnitOfWork unitOffWork,
    ILogger logger
)
    : IBaseService<TEntity, TCreateRequest, TUpdateRequest, TViewModel, TKey>
    where TEntity : BaseEntity<TKey>, new()
    where TCreateRequest : BaseCreateRequest, new()
    where TUpdateRequest : BaseUpdateRequest<TKey>, new()
    where TViewModel : BaseViewModel<TKey>, new()
{
    protected readonly IMapper Mapper = mapper;
    protected readonly IUnitOfWork UnitOffWork = unitOffWork;

    /// <summary>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<int?> DeleteHardAsync(TKey id)
    {
        logger.LogTrace($"{nameof(DeleteHardAsync)}: {id.TryParseToString()}");
        await UnitOffWork.Repository<TEntity, TKey>().DeleteHardAsync(id!);
        return await UnitOffWork.SaveChangesAsync();
    }

    /// <summary>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<int?> DeleteSoftAsync(TKey id)
    {
        logger.LogTrace($"{nameof(DeleteSoftAsync)}: {id.TryParseToString()}");
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
        logger.LogTrace($"{nameof(GetAllDtoAsync)} result: {result.TryParseToString()}");
        return result;
    }

    public virtual async Task<TViewModel?> GetByIdAsync(TKey id)
    {
        logger.LogTrace($"{nameof(GetByIdAsync)}: {id.TryParseToString()}");
        var entity = await UnitOffWork.Repository<TEntity, TKey>().GetByIdNoTrackingAsync(id);
        var result = Mapper.Map<TViewModel>(entity);
        logger.LogTrace($"{nameof(GetByIdAsync)} result: {result.TryParseToString()}");
        return result;
    }

    public virtual async Task<TViewModel?> UpdateAsync(TKey id, TUpdateRequest request)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            logger.LogTrace($"{nameof(UpdateAsync)} request: {id}, {request.TryParseToString()}");
            if (id is null || !id.Equals(request.Id))
                throw new KeyNotFoundException();
            var entity = await UnitOffWork.Repository<TEntity, TKey>().GetByIdAsync(id);
            logger.LogTrace($"{nameof(UpdateAsync)} old entity: {entity.TryParseToString()}");

            if (entity == null) throw new NullReferenceException();
            entity = Mapper.Map(request, entity);
            logger.LogTrace($"{nameof(UpdateAsync)} new entity: {entity.TryParseToString()}");
            var effectedCount = await UnitOffWork.Repository<TEntity, TKey>().UpdateAsync(entity);
            logger.LogTrace($"{nameof(UpdateAsync)} effectedCount: {effectedCount}");
            if (effectedCount <= 0) throw new UpdateFailedException();
            var result = Mapper.Map<TViewModel>(entity);
            logger.LogTrace($"{nameof(UpdateAsync)} result: {result.TryParseToString()}");
            await trans.CommitAsync();
            return result;
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public virtual async Task<TViewModel?> CreateAsync(TCreateRequest request)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            logger.LogTrace($"{nameof(CreateAsync)} request: {request.TryParseToString()}");
            var entityNew = new TEntity();
            Mapper.Map(request, entityNew);
            logger.LogTrace($"{nameof(CreateAsync)} entitiesNew: {entityNew.TryParseToString()}");
            var effectedCount =
                await UnitOffWork.Repository<TEntity, TKey>().CreateAsync(entityNew);
            logger.LogTrace($"{nameof(CreateAsync)} affectedCount: {effectedCount}");
            if (effectedCount <= 0) throw new CreateFailedException();
            var result = Mapper.Map<TViewModel>(entityNew);
            logger.LogTrace($"{nameof(CreateAsync)} result: {result.TryParseToString()}");
            await trans.CommitAsync();
            return result;
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
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

        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            var baseCreateRequests = request as TCreateRequest[] ?? request.ToArray();
            logger.LogTrace($"{nameof(CreateAsync)} request: {baseCreateRequests.TryParseToString()}");

            var entitiesNew = new List<TEntity>();
            Mapper.Map(baseCreateRequests, entitiesNew);
            logger.LogTrace($"{nameof(CreateAsync)} entitiesNew: {entitiesNew.TryParseToString()}");

            var effectedCount =
                await UnitOffWork.Repository<TEntity, TKey>().CreateAsync(entitiesNew);
            logger.LogTrace($"{nameof(CreateAsync)} affectedCount: {effectedCount}");

            if (effectedCount <= 0) throw new CreateFailedException();

            var result = Mapper.Map<IEnumerable<TViewModel>>(entitiesNew);
            var baseViewModels = result as TViewModel[] ?? result.ToArray();
            logger.LogTrace($"{nameof(CreateAsync)} result: {baseViewModels.TryParseToString()}");
            await trans.CommitAsync();

            return baseViewModels;
        }
        catch
        {
            await trans.CommitAsync();
            throw;
        }
    }
}