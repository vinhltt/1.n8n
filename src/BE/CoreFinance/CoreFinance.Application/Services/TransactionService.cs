using AutoMapper;
using CoreFinance.Application.DTOs;
using CoreFinance.Application.Interfaces;
using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Contracts.EntityFrameworkUtilities;
using CoreFinance.Domain;
using CoreFinance.Domain.UnitOfWorks;
using Microsoft.Extensions.Logging;

namespace CoreFinance.Application.Services;

public class TransactionService(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ILogger<TransactionService> logger)
    : BaseService<Transaction, TransactionCreateRequest, TransactionUpdateRequest, TransactionViewModel, Guid>(mapper, unitOfWork, logger),
        ITransactionService
{
    // Add custom logic for Transaction if needed

    public async Task<IBasePaging<TransactionViewModel>?> GetPagingAsync(IFilterBodyRequest request)
    {
        var query =
            Mapper.ProjectTo<TransactionViewModel>(UnitOffWork.Repository<Transaction, Guid>()
                .GetNoTrackingEntities());

        // Example: filter by Description or other fields if needed
        if (!string.IsNullOrEmpty(request.SearchValue))
            query = query.Where(e => e.Description != null 
                                     && (e.Description.ToLower().Contains(request.SearchValue.ToLower()) 
                                         || e.CategorySummary!.ToLower().Contains(request.SearchValue.ToLower())));

        return await query.ToPagingAsync(request);
    }
}
