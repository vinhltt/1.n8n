using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain.Enums;

namespace CoreFinance.Application.DTOs.RecurringTransactionTemplate;

public class RecurringTransactionTemplateUpdateRequest : BaseUpdateRequest<Guid>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public RecurringTransactionType? TransactionType { get; set; }
    public string? Category { get; set; }
    public RecurrenceFrequency? Frequency { get; set; }
    public int? CustomIntervalDays { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CronExpression { get; set; }
    public bool? IsActive { get; set; }
    public bool? AutoGenerate { get; set; }
    public int? DaysInAdvance { get; set; }
    public string? Notes { get; set; }
}