using CoreFinance.Application.DTOs.Transaction;
using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain;

namespace CoreFinance.Application.Interfaces;

public interface ITransactionService :
    IBaseService<Transaction, TransactionCreateRequest, TransactionUpdateRequest, TransactionViewModel, Guid>
{
    // Add custom methods for Transaction if needed
    Task<IBasePaging<TransactionViewModel>?> GetPagingAsync(IFilterBodyRequest request);
}