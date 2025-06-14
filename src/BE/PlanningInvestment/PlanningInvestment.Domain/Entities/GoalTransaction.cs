using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlanningInvestment.Domain.Enums;
using Shared.Contracts.BaseEfModels;

namespace PlanningInvestment.Domain.Entities;

/// <summary>
/// Represents a goal transaction entity for tracking goal contributions and withdrawals. (EN)<br/>
/// Đại diện cho thực thể giao dịch mục tiêu để theo dõi đóng góp và rút tiền mục tiêu. (VI)
/// </summary>
public class GoalTransaction : BaseEntity<Guid>
{
    /// <summary>
    /// Foreign key linking to goal (EN)<br/>
    /// Khóa ngoại liên kết với mục tiêu (VI)
    /// </summary>
    [Required]
    public Guid GoalId { get; set; }

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
    public GoalTransactionType Type { get; set; }

    /// <summary>
    /// Description of the transaction (EN)<br/>
    /// Mô tả về giao dịch (VI)
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property to goal (EN)<br/>
    /// Thuộc tính điều hướng đến mục tiêu (VI)
    /// </summary>
    public virtual Goal Goal { get; set; } = null!;
}
