using AutoMapper;
using CoreFinance.Application.DTOs.ExpectedTransaction;
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

public class ExpectedTransactionService(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ILogger<ExpectedTransactionService> logger)
    : BaseService<ExpectedTransaction, ExpectedTransactionCreateRequest,
            ExpectedTransactionUpdateRequest, ExpectedTransactionViewModel, Guid>(mapper, unitOfWork, logger),
        IExpectedTransactionService
{
    public async Task<IBasePaging<ExpectedTransactionViewModel>?> GetPagingAsync(IFilterBodyRequest request)
    {
        //var test = UnitOffWork.Repository<ExpectedTransaction, Guid>()
        //    .GetNoTrackingEntities();
        var query = Mapper.ProjectTo<ExpectedTransactionViewModel>(
            UnitOffWork.Repository<ExpectedTransaction, Guid>()
                .GetNoTrackingEntities());

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(e =>
                (e.Description != null && e.Description.ToLower().Contains(request.SearchValue.ToLower())) ||
                (e.Category != null && e.Category.ToLower().Contains(request.SearchValue.ToLower())) ||
                (e.TemplateName != null && e.TemplateName.ToLower().Contains(request.SearchValue.ToLower())));
        }

        return await query.ToPagingAsync(request);
    }

    public async Task<IEnumerable<ExpectedTransactionViewModel>> GetPendingTransactionsAsync(Guid userId)
    {
        var query = UnitOffWork.Repository<ExpectedTransaction, Guid>()
            .GetNoTrackingEntities()
            .Where(t => t.UserId == userId && t.Status == ExpectedTransactionStatus.Pending);

        return await Mapper.ProjectTo<ExpectedTransactionViewModel>(query).ToListAsync();
    }

    public async Task<IEnumerable<ExpectedTransactionViewModel>> GetUpcomingTransactionsAsync(Guid userId,
        int days = 30)
    {
        var endDate = DateTime.UtcNow.AddDays(days);
        var query = UnitOffWork.Repository<ExpectedTransaction, Guid>()
            .GetNoTrackingEntities()
            .Where(t => t.UserId == userId &&
                        t.Status == ExpectedTransactionStatus.Pending &&
                        t.ExpectedDate >= DateTime.UtcNow.Date &&
                        t.ExpectedDate <= endDate)
            .OrderBy(t => t.ExpectedDate);

        return await Mapper.ProjectTo<ExpectedTransactionViewModel>(query).ToListAsync();
    }

    public async Task<IEnumerable<ExpectedTransactionViewModel>> GetTransactionsByTemplateAsync(Guid templateId)
    {
        var query = UnitOffWork.Repository<ExpectedTransaction, Guid>()
            .GetNoTrackingEntities()
            .Where(t => t.RecurringTransactionTemplateId == templateId)
            .OrderBy(t => t.ExpectedDate);

        return await Mapper.ProjectTo<ExpectedTransactionViewModel>(query).ToListAsync();
    }

    public async Task<IEnumerable<ExpectedTransactionViewModel>> GetTransactionsByAccountAsync(Guid accountId)
    {
        var query = UnitOffWork.Repository<ExpectedTransaction, Guid>()
            .GetNoTrackingEntities()
            .Where(t => t.AccountId == accountId)
            .OrderBy(t => t.ExpectedDate);

        return await Mapper.ProjectTo<ExpectedTransactionViewModel>(query).ToListAsync();
    }

    public async Task<IEnumerable<ExpectedTransactionViewModel>> GetTransactionsByDateRangeAsync(Guid userId,
        DateTime startDate, DateTime endDate)
    {
        var query = UnitOffWork.Repository<ExpectedTransaction, Guid>()
            .GetNoTrackingEntities()
            .Where(t => t.UserId == userId &&
                        t.ExpectedDate >= startDate.Date &&
                        t.ExpectedDate <= endDate.Date)
            .OrderBy(t => t.ExpectedDate);

        return await Mapper.ProjectTo<ExpectedTransactionViewModel>(query).ToListAsync();
    }

    public async Task<bool> ConfirmExpectedTransactionAsync(Guid expectedTransactionId, Guid actualTransactionId)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            var expectedTransaction = await UnitOffWork.Repository<ExpectedTransaction, Guid>()
                .GetByIdAsync(expectedTransactionId);

            if (expectedTransaction == null || expectedTransaction.Status != ExpectedTransactionStatus.Pending)
                return false;

            expectedTransaction.Status = ExpectedTransactionStatus.Confirmed;
            expectedTransaction.ActualTransactionId = actualTransactionId;
            expectedTransaction.ProcessedAt = DateTime.UtcNow;
            expectedTransaction.UpdatedAt = DateTime.UtcNow;

            await UnitOffWork.Repository<ExpectedTransaction, Guid>().UpdateAsync(expectedTransaction);
            await UnitOffWork.SaveChangesAsync();

            await trans.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            logger.LogError(ex, "Error confirming expected transaction {ExpectedTransactionId}", expectedTransactionId);
            return false;
        }
    }

    public async Task<bool> CancelExpectedTransactionAsync(Guid expectedTransactionId, string reason)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            var expectedTransaction = await UnitOffWork.Repository<ExpectedTransaction, Guid>()
                .GetByIdAsync(expectedTransactionId);

            if (expectedTransaction == null || expectedTransaction.Status != ExpectedTransactionStatus.Pending)
                return false;

            expectedTransaction.Status = ExpectedTransactionStatus.Cancelled;
            expectedTransaction.AdjustmentReason = reason;
            expectedTransaction.ProcessedAt = DateTime.UtcNow;
            expectedTransaction.UpdatedAt = DateTime.UtcNow;

            await UnitOffWork.Repository<ExpectedTransaction, Guid>().UpdateAsync(expectedTransaction);
            await UnitOffWork.SaveChangesAsync();

            await trans.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            logger.LogError(ex, "Error cancelling expected transaction {ExpectedTransactionId}", expectedTransactionId);
            return false;
        }
    }

    public async Task<bool> AdjustExpectedTransactionAsync(Guid expectedTransactionId, decimal newAmount, string reason)
    {
        await using var trans = await UnitOffWork.BeginTransactionAsync();
        try
        {
            var expectedTransaction = await UnitOffWork.Repository<ExpectedTransaction, Guid>()
                .GetByIdAsync(expectedTransactionId);

            if (expectedTransaction == null || expectedTransaction.Status != ExpectedTransactionStatus.Pending)
                return false;

            if (!expectedTransaction.IsAdjusted)
            {
                expectedTransaction.OriginalAmount = expectedTransaction.ExpectedAmount;
            }

            expectedTransaction.ExpectedAmount = newAmount;
            expectedTransaction.IsAdjusted = true;
            expectedTransaction.AdjustmentReason = reason;
            expectedTransaction.UpdatedAt = DateTime.UtcNow;

            await UnitOffWork.Repository<ExpectedTransaction, Guid>().UpdateAsync(expectedTransaction);
            await UnitOffWork.SaveChangesAsync();

            await trans.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            logger.LogError(ex, "Error adjusting expected transaction {ExpectedTransactionId}", expectedTransactionId);
            return false;
        }
    }

    public async Task<decimal> GetCashFlowForecastAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        var expectedTransactions = (await GetTransactionsByDateRangeAsync(userId, startDate, endDate)).ToList();

        var totalIncome = expectedTransactions
            .Where(t => t is
                { TransactionType: RecurringTransactionType.Income, Status: ExpectedTransactionStatus.Pending })
            .Sum(t => t.ExpectedAmount);

        var totalExpenses = expectedTransactions
            .Where(t => t is
                { TransactionType: RecurringTransactionType.Expense, Status: ExpectedTransactionStatus.Pending })
            .Sum(t => t.ExpectedAmount);

        return totalIncome - totalExpenses;
    }

    public async Task<Dictionary<string, decimal>> GetCategoryForecastAsync(Guid userId, DateTime startDate,
        DateTime endDate)
    {
        var expectedTransactions = await GetTransactionsByDateRangeAsync(userId, startDate, endDate);

        return expectedTransactions
            .Where(t => t.Status == ExpectedTransactionStatus.Pending && !string.IsNullOrEmpty(t.Category))
            .GroupBy(t => t.Category!)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t =>
                    t.TransactionType == RecurringTransactionType.Income ? t.ExpectedAmount : -t.ExpectedAmount)
            );
    }

    public override async Task<ExpectedTransactionViewModel?> CreateAsync(ExpectedTransactionCreateRequest request)
    {
        // Set default values
        if (request.Status == default)
        {
            request.Status = ExpectedTransactionStatus.Pending;
        }

        return await base.CreateAsync(request);
    }
}