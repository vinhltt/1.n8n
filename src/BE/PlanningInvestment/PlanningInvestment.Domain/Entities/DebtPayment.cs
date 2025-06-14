using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlanningInvestment.Domain.Enums;
using Shared.Contracts.BaseEfModels;

namespace PlanningInvestment.Domain.Entities;

/// <summary>
/// Represents a debt payment entity for tracking debt payments. (EN)<br/>
/// Đại diện cho thực thể thanh toán nợ để theo dõi các khoản thanh toán nợ. (VI)
/// </summary>
public class DebtPayment : BaseEntity<Guid>
{
    /// <summary>
    /// Foreign key linking to debt (EN)<br/>
    /// Khóa ngoại liên kết với khoản nợ (VI)
    /// </summary>
    [Required]
    public Guid DebtId { get; set; }

    /// <summary>
    /// Total payment amount (EN)<br/>
    /// Tổng số tiền thanh toán (VI)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Date when payment was made (EN)<br/>
    /// Ngày thực hiện thanh toán (VI)
    /// </summary>
    [Required]
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// Principal amount portion of payment (EN)<br/>
    /// Phần số tiền gốc trong khoản thanh toán (VI)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99, ErrorMessage = "Principal amount must be greater than or equal to 0")]
    public decimal? PrincipalAmount { get; set; }

    /// <summary>
    /// Interest amount portion of payment (EN)<br/>
    /// Phần số tiền lãi trong khoản thanh toán (VI)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99, ErrorMessage = "Interest amount must be greater than or equal to 0")]
    public decimal? InterestAmount { get; set; }

    /// <summary>
    /// Description of the payment (EN)<br/>
    /// Mô tả về khoản thanh toán (VI)
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Status of the payment (EN)<br/>
    /// Trạng thái của khoản thanh toán (VI)
    /// </summary>
    [Required]
    public DebtPaymentStatus Status { get; set; } = DebtPaymentStatus.Completed;

    /// <summary>
    /// Navigation property to debt (EN)<br/>
    /// Thuộc tính điều hướng đến khoản nợ (VI)
    /// </summary>
    public virtual Debt Debt { get; set; } = null!;

    /// <summary>
    /// Calculated property: Other fees (difference between total and principal + interest) (EN)<br/>
    /// Thuộc tính tính toán: Phí khác (chênh lệch giữa tổng và gốc + lãi) (VI)
    /// </summary>
    [NotMapped]
    public decimal? OtherFees
    {
        get
        {
            if (!PrincipalAmount.HasValue || !InterestAmount.HasValue)
                return null;
            return Amount - (PrincipalAmount.Value + InterestAmount.Value);
        }
    }
}
