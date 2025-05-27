using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain.Enums;

namespace CoreFinance.Application.DTOs.ExpectedTransaction;

public class ExpectedTransactionCreateRequest : BaseCreateRequest
{
    public Guid RecurringTransactionTemplateId { get; set; }
    public Guid? UserId { get; set; }
    public Guid AccountId { get; set; }
    public DateTime ExpectedDate { get; set; }
    public decimal ExpectedAmount { get; set; }
    public string? Description { get; set; }
    public RecurringTransactionType TransactionType { get; set; }
    public string? Category { get; set; }
    public ExpectedTransactionStatus Status { get; set; } = ExpectedTransactionStatus.Pending;
    public string? Notes { get; set; }
}