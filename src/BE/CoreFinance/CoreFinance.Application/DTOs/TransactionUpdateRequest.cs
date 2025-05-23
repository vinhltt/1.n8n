using System;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain;

namespace CoreFinance.Application.DTOs;

/// <summary>
/// DTO for updating a transaction.
/// </summary>
public class TransactionUpdateRequest : BaseUpdateRequest<Guid>
{
    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal RevenueAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public string? Description { get; set; }
    public decimal Balance { get; set; }
    public decimal? BalanceCompare { get; set; }
    public decimal? AvailableLimit { get; set; }
    public decimal? AvailableLimitCompare { get; set; }
    public string? TransactionCode { get; set; }
    public bool SyncMisa { get; set; }
    public bool SyncSms { get; set; }
    public bool Vn { get; set; }
    public string? CategorySummary { get; set; }
    public string? Note { get; set; }
    public string? ImportFrom { get; set; }
    public decimal? IncreaseCreditLimit { get; set; }
    public decimal? UsedPercent { get; set; }
    public CategoryType CategoryType { get; set; }
    public string? Group { get; set; }
}
