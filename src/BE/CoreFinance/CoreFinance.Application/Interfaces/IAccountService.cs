using CoreFinance.Application.DTOs.Account;
using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain;

namespace CoreFinance.Application.Interfaces;

public interface
    IAccountService : IBaseService<Account, AccountCreateRequest, AccountUpdateRequest, AccountViewModel, Guid>
{
    Task<IBasePaging<AccountViewModel>?> GetPagingAsync(IFilterBodyRequest request);
}