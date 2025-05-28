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
/// Base service class providing common CRUD operations for entities. (EN)
/// <br/>
/// Lớp dịch vụ cơ bản cung cấp các thao tác CRUD chung cho các thực thể. (VI)
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TCreateRequest">The type of the create request DTO.</typeparam>
/// <typeparam name="TUpdateRequest">The type of the update request DTO.</typeparam>
/// <typeparam name="TViewModel">The type of the view model DTO.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
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
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Deletes an entity permanently by its identifier asynchronously. (EN)
    /// <br/>
    /// Xóa vĩnh viễn một thực thể dựa trên định danh của nó một cách bất đồng bộ. (VI)
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    public async Task<int?> DeleteHardAsync(TKey id)
    {
        _logger.LogTrace($"{nameof(DeleteHardAsync)}: {id.TryParseToString()}");
        return await UnitOffWork.Repository<TEntity, TKey>().DeleteHardAsync(id!);
    }

    /// <summary>
    /// Soft deletes an entity by its identifier asynchronously. (EN)
    /// <br/>
    /// Xóa mềm một thực thể dựa trên định danh của nó một cách bất đồng bộ. (VI)
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    public async Task<int?> DeleteSoftAsync(TKey id)
    {
        _logger.LogTrace($"{nameof(DeleteSoftAsync)}: {id.TryParseToString()}");
        return await UnitOffWork.Repository<TEntity, TKey>().DeleteSoftAsync(id!);
    }

    /// <summary>
    /// Gets all entities as view models asynchronously. (EN)
    /// <br/>
    /// Lấy tất cả các thực thể dưới dạng view model một cách bất đồng bộ. (VI)
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of view models.</returns>
    public virtual async Task<IEnumerable<TViewModel>?> GetAllDtoAsync()
    {
        var query = UnitOffWork.Repository<TEntity, TKey>();
        var result = await Mapper.ProjectTo<TViewModel>(query.GetNoTrackingEntities())
            .ToListAsync();
        _logger.LogTrace($"{nameof(GetAllDtoAsync)} result: {result.TryParseToString()}");
        return result;
    }

    /// <summary>
    /// Gets an entity by its identifier as a view model asynchronously. (EN)
    /// <br/>
    /// Lấy một thực thể dựa trên định danh của nó dưới dạng view model một cách bất đồng bộ. (VI)
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the view model, or null if the entity is not found.</returns>
    public virtual async Task<TViewModel?> GetByIdAsync(TKey id)
    {
        _logger.LogTrace($"{nameof(GetByIdAsync)}: {id.TryParseToString()}");
        var entity = await UnitOffWork.Repository<TEntity, TKey>().GetByIdNoTrackingAsync(id);
        var result = Mapper.Map<TViewModel>(entity);
        _logger.LogTrace($"{nameof(GetByIdAsync)} result: {result.TryParseToString()}");
        return result;
    }

    /// <summary>
    /// Updates an entity asynchronously. (EN)
    /// <br/>
    /// Cập nhật một thực thể một cách bất đồng bộ. (VI)
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="request">The update request containing the new data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated view model, or null if the update failed.</returns>
    public virtual async Task<TViewModel?> UpdateAsync(TKey id, TUpdateRequest request)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            _logger.LogTrace($"{nameof(UpdateAsync)} request: {id}, {request.TryParseToString()}");
            if (id is null || !id.Equals(request.Id))
                throw new KeyNotFoundException();
            var entity = await UnitOffWork.Repository<TEntity, TKey>().GetByIdAsync(id);
            _logger.LogTrace($"{nameof(UpdateAsync)} old entity: {entity.TryParseToString()}");

            if (entity == null) throw new NullReferenceException();
            entity = Mapper.Map(request, entity);
            _logger.LogTrace($"{nameof(UpdateAsync)} new entity: {entity.TryParseToString()}");
            var effectedCount = await UnitOffWork.Repository<TEntity, TKey>().UpdateAsync(entity);
            _logger.LogTrace($"{nameof(UpdateAsync)} effectedCount: {effectedCount}");
            if (effectedCount <= 0) throw new UpdateFailedException();
            var result = Mapper.Map<TViewModel>(entity);
            _logger.LogTrace($"{nameof(UpdateAsync)} result: {result.TryParseToString()}");
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
    /// Creates a new entity asynchronously. (EN)
    /// <br/>
    /// Tạo một thực thể mới một cách bất đồng bộ. (VI)
    /// </summary>
    /// <param name="request">The create request containing the data for the new entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created view model, or null if the creation failed.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public virtual async Task<TViewModel?> CreateAsync(TCreateRequest request)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            _logger.LogTrace($"{nameof(CreateAsync)} request: {request.TryParseToString()}");
            var entityNew = new TEntity();
            Mapper.Map(request, entityNew);
            _logger.LogTrace($"{nameof(CreateAsync)} entitiesNew: {entityNew.TryParseToString()}");
            var effectedCount =
                await UnitOffWork.Repository<TEntity, TKey>().CreateAsync(entityNew);
            _logger.LogTrace($"{nameof(CreateAsync)} affectedCount: {effectedCount}");
            if (effectedCount <= 0) throw new CreateFailedException();
            var result = Mapper.Map<TViewModel>(entityNew);
            _logger.LogTrace($"{nameof(CreateAsync)} result: {result.TryParseToString()}");
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
    /// Creates multiple new entities asynchronously. (EN)
    /// <br/>
    /// Tạo nhiều thực thể mới một cách bất đồng bộ. (VI)
    /// </summary>
    /// <param name="request">The list of create requests containing the data for the new entities.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of created view models, or an empty collection if the creation failed.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public virtual async Task<IEnumerable<TViewModel>?> CreateAsync(
        List<TCreateRequest> request)
    {
        if (!request.Any())
        {
            _logger.LogInformation("Request Is Empty");
            return new List<TViewModel>();
        }

        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            var baseCreateRequests = request as TCreateRequest[] ?? request.ToArray();
            _logger.LogTrace($"{nameof(CreateAsync)} request: {baseCreateRequests.TryParseToString()}");

            var entitiesNew = new List<TEntity>();
            Mapper.Map(baseCreateRequests, entitiesNew);
            _logger.LogTrace($"{nameof(CreateAsync)} entitiesNew: {entitiesNew.TryParseToString()}");

            var effectedCount =
                await UnitOffWork.Repository<TEntity, TKey>().CreateAsync(entitiesNew);
            _logger.LogTrace($"{nameof(CreateAsync)} affectedCount: {effectedCount}");

            if (effectedCount <= 0) throw new CreateFailedException();

            var result = Mapper.Map<IEnumerable<TViewModel>>(entitiesNew);
            var baseViewModels = result as TViewModel[] ?? result.ToArray();
            _logger.LogTrace($"{nameof(CreateAsync)} result: {baseViewModels.TryParseToString()}");
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