using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlanningInvestment.Domain.Enums;
using Shared.Contracts.BaseEfModels;

namespace PlanningInvestment.Domain.Entities;

/// <summary>
/// Represents an investment transaction entity for tracking investment activities. (EN)<br/>
/// Đại diện cho thực thể giao dịch đầu tư để theo dõi các hoạt động đầu tư. (VI)
/// </summary>
public class InvestmentTransaction : BaseEntity<Guid>
{
    /// <summary>
    /// Foreign key linking to investment (EN)<br/>
    /// Khóa ngoại liên kết với khoản đầu tư (VI)
    /// </summary>
    [Required]
    public Guid InvestmentId { get; set; }

    /// <summary>
    /// Transaction amount (EN)<br/>
    /// Số tiền giao dịch (VI)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Date when transaction occurred (EN)<br/>
    /// Ngày xảy ra giao dịch (VI)
    /// </summary>
    [Required]
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Type of transaction (EN)<br/>
    /// Loại giao dịch (VI)
    /// </summary>
    [Required]
    public InvestmentTransactionType Type { get; set; }

    /// <summary>
    /// Quantity of shares/units (EN)<br/>
    /// Số lượng cổ phiếu/đơn vị (VI)
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    [Range(0, 999999999.9999, ErrorMessage = "Quantity must be greater than or equal to 0")]
    public decimal? Quantity { get; set; }

    /// <summary>
    /// Unit price per share/unit (EN)<br/>
    /// Giá đơn vị mỗi cổ phiếu/đơn vị (VI)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99, ErrorMessage = "Unit price must be greater than or equal to 0")]
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Description of the transaction (EN)<br/>
    /// Mô tả về giao dịch (VI)
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property to investment (EN)<br/>
    /// Thuộc tính điều hướng đến khoản đầu tư (VI)
    /// </summary>
    public virtual Investment Investment { get; set; } = null!;

    /// <summary>
    /// Calculated property: Total transaction value (EN)<br/>
    /// Thuộc tính tính toán: Tổng giá trị giao dịch (VI)
    /// </summary>
    [NotMapped]
    public decimal? TotalValue => Quantity.HasValue && UnitPrice.HasValue ? Quantity.Value * UnitPrice.Value : null;
}
