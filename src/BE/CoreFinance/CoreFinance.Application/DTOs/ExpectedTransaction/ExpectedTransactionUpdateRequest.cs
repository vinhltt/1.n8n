using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain.Enums;

namespace CoreFinance.Application.DTOs.ExpectedTransaction;

public class ExpectedTransactionUpdateRequest : BaseUpdateRequest<Guid>
{
    public DateTime? ExpectedDate { get; set; }
    public decimal? ExpectedAmount { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public ExpectedTransactionStatus? Status { get; set; }
    public string? AdjustmentReason { get; set; }
    public string? Notes { get; set; }
}