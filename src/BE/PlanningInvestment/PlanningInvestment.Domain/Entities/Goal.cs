using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlanningInvestment.Domain.Enums;
using Shared.Contracts.BaseEfModels;

namespace PlanningInvestment.Domain.Entities;

/// <summary>
/// Represents a financial goal entity for goal management. (EN)<br/>
/// Đại diện cho thực thể mục tiêu tài chính để quản lý mục tiêu. (VI)
/// </summary>
public class Goal : BaseEntity<Guid>
{
    /// <summary>
    /// Foreign key linking to user (EN)<br/>
    /// Khóa ngoại liên kết với người dùng (VI)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Goal name for identification (EN)<br/>
    /// Tên mục tiêu để nhận biết (VI)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the goal (EN)<br/>
    /// Mô tả về mục tiêu (VI)
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Target amount to achieve (EN)<br/>
    /// Số tiền mục tiêu cần đạt được (VI)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99, ErrorMessage = "Target amount must be greater than 0")]
    public decimal TargetAmount { get; set; }

    /// <summary>
    /// Current amount saved (EN)<br/>
    /// Số tiền hiện tại đã tiết kiệm (VI)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99, ErrorMessage = "Current amount must be greater than or equal to 0")]
    public decimal CurrentAmount { get; set; } = 0;

    /// <summary>
    /// Start date of the goal (EN)<br/>
    /// Ngày bắt đầu mục tiêu (VI)
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Target date to achieve the goal (EN)<br/>
    /// Ngày mục tiêu để đạt được mục tiêu (VI)
    /// </summary>
    public DateTime? TargetDate { get; set; }

    /// <summary>
    /// Current status of the goal (EN)<br/>
    /// Trạng thái hiện tại của mục tiêu (VI)
    /// </summary>
    [Required]
    public GoalStatus Status { get; set; } = GoalStatus.Planning;

    /// <summary>
    /// Category of the goal (EN)<br/>
    /// Danh mục của mục tiêu (VI)
    /// </summary>
    [Required]
    public GoalCategory Category { get; set; }

    /// <summary>
    /// Priority level (0-100) (EN)<br/>
    /// Mức độ ưu tiên (0-100) (VI)
    /// </summary>
    [Range(0, 100, ErrorMessage = "Priority must be between 0 and 100")]
    public decimal Priority { get; set; } = 50;

    /// <summary>
    /// Monthly contribution amount (EN)<br/>
    /// Số tiền đóng góp hàng tháng (VI)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99, ErrorMessage = "Monthly contribution must be greater than or equal to 0")]
    public decimal MonthlyContribution { get; set; } = 0;

    /// <summary>
    /// Navigation property for goal transactions (EN)<br/>
    /// Thuộc tính điều hướng cho các giao dịch mục tiêu (VI)
    /// </summary>
    public virtual ICollection<GoalTransaction> GoalTransactions { get; set; } = new List<GoalTransaction>();

    /// <summary>
    /// Calculated property: Progress percentage (EN)<br/>
    /// Thuộc tính tính toán: Phần trăm tiến độ (VI)
    /// </summary>
    [NotMapped]
    public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;

    /// <summary>
    /// Calculated property: Days remaining to target date (EN)<br/>
    /// Thuộc tính tính toán: Số ngày còn lại đến ngày mục tiêu (VI)
    /// </summary>
    [NotMapped]
    public int? DaysRemaining => TargetDate?.Subtract(DateTime.UtcNow).Days;

    /// <summary>
    /// Calculated property: Whether goal is achieved (EN)<br/>
    /// Thuộc tính tính toán: Mục tiêu đã đạt được chưa (VI)
    /// </summary>
    [NotMapped]
    public bool IsAchieved => CurrentAmount >= TargetAmount;

    /// <summary>
    /// Calculated property: Amount needed to complete goal (EN)<br/>
    /// Thuộc tính tính toán: Số tiền cần để hoàn thành mục tiêu (VI)
    /// </summary>
    [NotMapped]
    public decimal AmountNeeded => Math.Max(0, TargetAmount - CurrentAmount);
}
