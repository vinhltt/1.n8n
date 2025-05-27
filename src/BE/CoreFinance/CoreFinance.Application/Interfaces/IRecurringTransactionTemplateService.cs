using CoreFinance.Application.DTOs.RecurringTransactionTemplate;
using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain;

namespace CoreFinance.Application.Interfaces;

public interface IRecurringTransactionTemplateService : IBaseService<RecurringTransactionTemplate,
    RecurringTransactionTemplateCreateRequest, RecurringTransactionTemplateUpdateRequest,
    RecurringTransactionTemplateViewModel, Guid>
{
    Task<IBasePaging<RecurringTransactionTemplateViewModel>?> GetPagingAsync(IFilterBodyRequest request);
    Task<IEnumerable<RecurringTransactionTemplateViewModel>> GetActiveTemplatesAsync(Guid userId);
    Task<IEnumerable<RecurringTransactionTemplateViewModel>> GetTemplatesByAccountAsync(Guid accountId);
    Task<bool> ToggleActiveStatusAsync(Guid templateId, bool isActive);
    Task<DateTime> CalculateNextExecutionDateAsync(Guid templateId);
    Task GenerateExpectedTransactionsAsync(Guid templateId, int daysInAdvance);
    Task GenerateExpectedTransactionsForAllActiveTemplatesAsync();
}