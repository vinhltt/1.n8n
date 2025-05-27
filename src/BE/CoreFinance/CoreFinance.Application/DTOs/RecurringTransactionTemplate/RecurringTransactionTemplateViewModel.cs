using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain.Enums;

namespace CoreFinance.Application.DTOs.RecurringTransactionTemplate;

public class RecurringTransactionTemplateViewModel : BaseViewModel<Guid>
{
    public Guid? UserId { get; set; }
    public Guid AccountId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public RecurringTransactionType TransactionType { get; set; }
    public string? Category { get; set; }
    public RecurrenceFrequency Frequency { get; set; }
    public int? CustomIntervalDays { get; set; }
    public DateTime NextExecutionDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CronExpression { get; set; }
    public bool IsActive { get; set; }
    public bool AutoGenerate { get; set; }
    public int DaysInAdvance { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public string? AccountName { get; set; }
    public AccountType? AccountType { get; set; }
    public int ExpectedTransactionCount { get; set; }
}