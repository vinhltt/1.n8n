using AutoMapper;
using CoreFinance.Application.DTOs.RecurringTransactionTemplate;
using CoreFinance.Application.Interfaces;
using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Contracts.EntityFrameworkUtilities;
using CoreFinance.Domain;
using CoreFinance.Domain.Enums;
using CoreFinance.Domain.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreFinance.Application.Services;

public class RecurringTransactionTemplateService(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ILogger<RecurringTransactionTemplateService> logger)
    : BaseService<RecurringTransactionTemplate, RecurringTransactionTemplateCreateRequest,
            RecurringTransactionTemplateUpdateRequest, RecurringTransactionTemplateViewModel, Guid>(mapper, unitOfWork,
            logger),
        IRecurringTransactionTemplateService
{
    public async Task<IBasePaging<RecurringTransactionTemplateViewModel>?> GetPagingAsync(IFilterBodyRequest request)
    {
        var query = Mapper.ProjectTo<RecurringTransactionTemplateViewModel>(
            UnitOffWork.Repository<RecurringTransactionTemplate, Guid>()
                .GetNoTrackingEntities());

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(t => (t.Name != null && t.Name.ToLower().Contains(request.SearchValue.ToLower())) ||
                                     (t.Description != null &&
                                      t.Description.ToLower().Contains(request.SearchValue.ToLower())) ||
                                     (t.Category != null &&
                                      t.Category.ToLower().Contains(request.SearchValue.ToLower())));
        }

        return await query.ToPagingAsync(request);
    }

    public async Task<IEnumerable<RecurringTransactionTemplateViewModel>> GetActiveTemplatesAsync(Guid userId)
    {
        var query = UnitOffWork.Repository<RecurringTransactionTemplate, Guid>()
            .GetNoTrackingEntities()
            .Where(t => t.UserId == userId && t.IsActive);

        return await Mapper.ProjectTo<RecurringTransactionTemplateViewModel>(query).ToListAsync();
    }

    public async Task<IEnumerable<RecurringTransactionTemplateViewModel>> GetTemplatesByAccountAsync(Guid accountId)
    {
        var query = UnitOffWork.Repository<RecurringTransactionTemplate, Guid>()
            .GetNoTrackingEntities()
            .Where(t => t.AccountId == accountId);

        return await Mapper.ProjectTo<RecurringTransactionTemplateViewModel>(query).ToListAsync();
    }

    public async Task<bool> ToggleActiveStatusAsync(Guid templateId, bool isActive)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            var template = await UnitOffWork.Repository<RecurringTransactionTemplate, Guid>()
                .GetByIdAsync(templateId);

            if (template == null)
                return false;

            template.IsActive = isActive;
            template.UpdatedAt = DateTime.UtcNow;

            await UnitOffWork.Repository<RecurringTransactionTemplate, Guid>().UpdateAsync(template);
            await UnitOffWork.SaveChangesAsync();

            await trans.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            logger.LogError(ex, "Error toggling active status for template {TemplateId}", templateId);
            return false;
        }
    }

    public async Task<DateTime> CalculateNextExecutionDateAsync(Guid templateId)
    {
        var template = await UnitOffWork.Repository<RecurringTransactionTemplate, Guid>()
            .GetByIdAsync(templateId);

        if (template == null)
            throw new ArgumentException("Template not found", nameof(templateId));

        return CalculateNextExecutionDate(template.NextExecutionDate, template.Frequency, template.CustomIntervalDays);
    }

    public async Task GenerateExpectedTransactionsAsync(Guid templateId, int daysInAdvance)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            await GenerateExpectedTransactionsInternalAsync(templateId, daysInAdvance);
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            logger.LogError(ex, "Error generating expected transactions for template {TemplateId}", templateId);
            throw;
        }
    }

    private async Task GenerateExpectedTransactionsInternalAsync(Guid templateId, int daysInAdvance)
    {
        var template = await UnitOffWork.Repository<RecurringTransactionTemplate, Guid>()
            .GetByIdAsync(templateId);

        if (template == null || !template.IsActive || !template.AutoGenerate)
            return;

        var endDate = DateTime.UtcNow.AddDays(daysInAdvance);
        var currentDate = template.NextExecutionDate;

        // Check if end date is set and we've passed it
        if (template.EndDate.HasValue && currentDate > template.EndDate.Value)
            return;

        var expectedTransactionRepo = UnitOffWork.Repository<ExpectedTransaction, Guid>();

        while (currentDate <= endDate)
        {
            // Check if end date is set and we've passed it
            if (template.EndDate.HasValue && currentDate > template.EndDate.Value)
                break;

            // Check if expected transaction already exists for this date
            var existingTransaction = expectedTransactionRepo
                .GetNoTrackingEntities()
                .FirstOrDefault(et => et.RecurringTransactionTemplateId == templateId &&
                                      et.ExpectedDate.Date == currentDate.Date);

            if (existingTransaction == null)
            {
                var expectedTransaction = new ExpectedTransaction
                {
                    Id = Guid.NewGuid(),
                    RecurringTransactionTemplateId = templateId,
                    UserId = template.UserId,
                    AccountId = template.AccountId,
                    ExpectedDate = currentDate,
                    ExpectedAmount = template.Amount,
                    Description = template.Description,
                    TransactionType = template.TransactionType,
                    Category = template.Category,
                    Status = ExpectedTransactionStatus.Pending,
                    GeneratedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await expectedTransactionRepo.CreateAsync(expectedTransaction);
            }

            // Calculate next execution date
            currentDate = CalculateNextExecutionDate(currentDate, template.Frequency, template.CustomIntervalDays);
        }

        // Update template's next execution date
        template.NextExecutionDate = currentDate;
        template.UpdatedAt = DateTime.UtcNow;
        await UnitOffWork.Repository<RecurringTransactionTemplate, Guid>().UpdateAsync(template);

        await UnitOffWork.SaveChangesAsync();
    }

    public async Task GenerateExpectedTransactionsForAllActiveTemplatesAsync()
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            var activeTemplates = UnitOffWork.Repository<RecurringTransactionTemplate, Guid>()
                .GetNoTrackingEntities()
                .Where(t => t.IsActive && t.AutoGenerate)
                .ToList();

            foreach (var template in activeTemplates)
            {
                await GenerateExpectedTransactionsInternalAsync(template.Id, template.DaysInAdvance);
            }

            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            logger.LogError(ex, "Error generating expected transactions for all active templates");
            throw;
        }
    }

    private static DateTime CalculateNextExecutionDate(DateTime currentDate, RecurrenceFrequency frequency,
        int? customIntervalDays)
    {
        return frequency switch
        {
            RecurrenceFrequency.Daily => currentDate.AddDays(1),
            RecurrenceFrequency.Weekly => currentDate.AddDays(7),
            RecurrenceFrequency.Biweekly => currentDate.AddDays(14),
            RecurrenceFrequency.Monthly => currentDate.AddMonths(1),
            RecurrenceFrequency.Quarterly => currentDate.AddMonths(3),
            RecurrenceFrequency.SemiAnnually => currentDate.AddMonths(6),
            RecurrenceFrequency.Annually => currentDate.AddYears(1),
            RecurrenceFrequency.Custom => currentDate.AddDays(customIntervalDays ?? 1),
            _ => currentDate.AddDays(1)
        };
    }

    public override async Task<RecurringTransactionTemplateViewModel?> CreateAsync(
        RecurringTransactionTemplateCreateRequest request)
    {
        // Set default values
        if (request.NextExecutionDate == default)
        {
            request.NextExecutionDate = request.StartDate;
        }

        var result = await base.CreateAsync(request);

        // Generate expected transactions if auto-generate is enabled
        if (result != null && request.AutoGenerate)
        {
            await GenerateExpectedTransactionsAsync(result.Id, request.DaysInAdvance);
        }

        return result;
    }

    public override async Task<RecurringTransactionTemplateViewModel?> UpdateAsync(Guid id,
        RecurringTransactionTemplateUpdateRequest request)
    {
        var result = await base.UpdateAsync(id, request);

        // Regenerate expected transactions if auto-generate is enabled
        if (result is { AutoGenerate: true })
        {
            await GenerateExpectedTransactionsAsync(result.Id, result.DaysInAdvance);
        }

        return result;
    }
}