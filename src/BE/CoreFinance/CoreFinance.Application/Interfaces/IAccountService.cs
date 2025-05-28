using CoreFinance.Application.DTOs.Account;
using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain.Entities;

namespace CoreFinance.Application.Interfaces;

/// <summary>
/// Represents the service interface for managing accounts.<br/>(EN) Represents the service interface for managing accounts.<br/>(VI) Đại diện cho interface dịch vụ quản lý tài khoản.
/// </summary>
public interface
    IAccountService : IBaseService<Account, AccountCreateRequest, AccountUpdateRequest, AccountViewModel, Guid>
{
    /// <summary>
    /// Gets a paginated list of accounts.<br/>(EN) Gets a paginated list of accounts.<br/>(VI) Lấy danh sách tài khoản có phân trang.
    /// </summary>
    /// <param name="request">The filter request body.</param>
    /// <returns>A paginated list of accounts.</returns>
    Task<IBasePaging<AccountViewModel>?> GetPagingAsync(IFilterBodyRequest request);
}