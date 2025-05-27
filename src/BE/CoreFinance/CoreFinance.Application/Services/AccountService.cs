using AutoMapper;
using CoreFinance.Application.DTOs.Account;
using CoreFinance.Application.Interfaces;
using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Contracts.EntityFrameworkUtilities;
using CoreFinance.Domain;
using CoreFinance.Domain.UnitOfWorks;
using Microsoft.Extensions.Logging;

namespace CoreFinance.Application.Services;

public class AccountService(
    IMapper mapper,
    IUnitOfWork unitOffWork,
    ILogger<AccountService> logger)
    : BaseService<Account, AccountCreateRequest, AccountUpdateRequest, AccountViewModel, Guid>(mapper, unitOffWork,
            logger),
        IAccountService
{
    public async Task<IBasePaging<AccountViewModel>?> GetPagingAsync(IFilterBodyRequest request)
    {
        var query =
            Mapper.ProjectTo<AccountViewModel>(UnitOffWork.Repository<Account, Guid>()
                .GetNoTrackingEntities());

        if (!string.IsNullOrEmpty(request.SearchValue))
            query = query.Where(e => e.Name!.Contains(request.SearchValue, StringComparison.CurrentCultureIgnoreCase));

        return await query.ToPagingAsync(request);
    }
}