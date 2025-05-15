using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;

namespace CoreFinance.Application.Services.Base;

// ReSharper disable once UnusedTypeParameter
public interface IBaseService<TEntity, TCreateRequest, in TUpdateRequest, TViewModel,
    in TKey>
    where TEntity : BaseEntity<TKey>?, new()
    where TCreateRequest : BaseCreateRequest, new()
    where TUpdateRequest : BaseUpdateRequest<TKey>, new()
    where TViewModel : BaseViewModel<TKey>, new()
{
    Task<IEnumerable<TViewModel>?> GetAllDtoAsync();
    Task<TViewModel?> GetByIdAsync(TKey id);
    Task<int?> DeleteHardAsync(TKey id);
    Task<int?> DeleteSoftAsync(TKey id);
    Task<TViewModel?> UpdateAsync(TKey id, TUpdateRequest request);
    Task<TViewModel?> CreateAsync(TCreateRequest request);
    Task<IEnumerable<TViewModel>?> CreateAsync(List<TCreateRequest> request);
}