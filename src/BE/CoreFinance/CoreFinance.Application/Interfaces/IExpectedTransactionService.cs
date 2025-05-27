using CoreFinance.Application.DTOs.ExpectedTransaction;
using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain;

namespace CoreFinance.Application.Interfaces;

public interface IExpectedTransactionService : IBaseService<ExpectedTransaction,
    ExpectedTransactionCreateRequest, ExpectedTransactionUpdateRequest,
    ExpectedTransactionViewModel, Guid>
{
    Task<IBasePaging<ExpectedTransactionViewModel>?> GetPagingAsync(IFilterBodyRequest request);
    Task<IEnumerable<ExpectedTransactionViewModel>> GetPendingTransactionsAsync(Guid userId);
    Task<IEnumerable<ExpectedTransactionViewModel>> GetUpcomingTransactionsAsync(Guid userId, int days = 30);
    Task<IEnumerable<ExpectedTransactionViewModel>> GetTransactionsByTemplateAsync(Guid templateId);
    Task<IEnumerable<ExpectedTransactionViewModel>> GetTransactionsByAccountAsync(Guid accountId);

    Task<IEnumerable<ExpectedTransactionViewModel>> GetTransactionsByDateRangeAsync(Guid userId, DateTime startDate,
        DateTime endDate);

    Task<bool> ConfirmExpectedTransactionAsync(Guid expectedTransactionId, Guid actualTransactionId);
    Task<bool> CancelExpectedTransactionAsync(Guid expectedTransactionId, string reason);
    Task<bool> AdjustExpectedTransactionAsync(Guid expectedTransactionId, decimal newAmount, string reason);
    Task<decimal> GetCashFlowForecastAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<Dictionary<string, decimal>> GetCategoryForecastAsync(Guid userId, DateTime startDate, DateTime endDate);
}