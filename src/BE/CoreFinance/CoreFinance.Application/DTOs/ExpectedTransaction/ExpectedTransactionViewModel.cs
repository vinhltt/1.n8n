using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain.Enums;

namespace CoreFinance.Application.DTOs.ExpectedTransaction;

public class ExpectedTransactionViewModel : BaseViewModel<Guid>
{
    public Guid RecurringTransactionTemplateId { get; set; }
    public Guid? UserId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? ActualTransactionId { get; set; }
    public DateTime ExpectedDate { get; set; }
    public decimal ExpectedAmount { get; set; }
    public string? Description { get; set; }
    public RecurringTransactionType TransactionType { get; set; }
    public string? Category { get; set; }
    public ExpectedTransactionStatus Status { get; set; }
    public bool IsAdjusted { get; set; }
    public decimal? OriginalAmount { get; set; }
    public string? AdjustmentReason { get; set; }
    public string? Notes { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public string? TemplateName { get; set; }
    public string? AccountName { get; set; }
    public AccountType? AccountType { get; set; }
    public bool HasActualTransaction { get; set; }
    public int DaysUntilDue { get; set; }
}