using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlanningInvestment.Domain.Enums;
using Shared.Contracts.BaseEfModels;

namespace PlanningInvestment.Domain.Entities;

/// <summary>
/// Represents an investment entity for investment management. (EN)<br/>
/// Đại diện cho thực thể đầu tư để quản lý đầu tư. (VI)
/// </summary>
public class Investment : BaseEntity<Guid>
{
    /// <summary>
    /// Foreign key linking to user (EN)<br/>
    /// Khóa ngoại liên kết với người dùng (VI)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Investment name for identification (EN)<br/>
    /// Tên khoản đầu tư để nhận biết (VI)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the investment (EN)<br/>
    /// Mô tả về khoản đầu tư (VI)
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Type of investment (EN)<br/>
    /// Loại đầu tư (VI)
    /// </summary>
    [Required]
    public InvestmentType Type { get; set; }

    /// <summary>
    /// Investment symbol or ticker (EN)<br/>
    /// Mã chứng khoán hoặc ký hiệu đầu tư (VI)
    /// </summary>
    [MaxLength(20)]
    public string? Symbol { get; set; }

    /// <summary>
    /// Initial investment amount (EN)<br/>
    /// Số tiền đầu tư ban đầu (VI)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99, ErrorMessage = "Initial amount must be greater than 0")]
    public decimal InitialAmount { get; set; }

    /// <summary>
    /// Current market value (EN)<br/>
    /// Giá trị thị trường hiện tại (VI)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99, ErrorMessage = "Current value must be greater than or equal to 0")]
    public decimal CurrentValue { get; set; }

    /// <summary>
    /// Date when investment was purchased (EN)<br/>
    /// Ngày mua khoản đầu tư (VI)
    /// </summary>
    [Required]
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// Date when investment was sold (EN)<br/>
    /// Ngày bán khoản đầu tư (VI)
    /// </summary>
    public DateTime? SaleDate { get; set; }

    /// <summary>
    /// Return rate percentage (EN)<br/>
    /// Tỷ suất sinh lời theo phần trăm (VI)
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    [Range(-100, 1000, ErrorMessage = "Return rate must be between -100 and 1000")]
    public decimal? ReturnRate { get; set; }

    /// <summary>
    /// Current status of investment (EN)<br/>
    /// Trạng thái hiện tại của khoản đầu tư (VI)
    /// </summary>
    [Required]
    public InvestmentStatus Status { get; set; } = InvestmentStatus.Active;

    /// <summary>
    /// Risk level from 1 (low) to 10 (high) (EN)<br/>
    /// Mức độ rủi ro từ 1 (thấp) đến 10 (cao) (VI)
    /// </summary>
    [Range(1, 10, ErrorMessage = "Risk level must be between 1 and 10")]
    public int RiskLevel { get; set; } = 5;

    /// <summary>
    /// Trading platform or broker (EN)<br/>
    /// Sàn giao dịch hoặc nhà môi giới (VI)
    /// </summary>
    [MaxLength(100)]
    public string? Platform { get; set; }

    /// <summary>
    /// Navigation property for investment transactions (EN)<br/>
    /// Thuộc tính điều hướng cho các giao dịch đầu tư (VI)
    /// </summary>
    public virtual ICollection<InvestmentTransaction> InvestmentTransactions { get; set; } = new List<InvestmentTransaction>();

    /// <summary>
    /// Calculated property: Gain or loss amount (EN)<br/>
    /// Thuộc tính tính toán: Số tiền lãi hoặc lỗ (VI)
    /// </summary>
    [NotMapped]
    public decimal GainLoss => CurrentValue - InitialAmount;

    /// <summary>
    /// Calculated property: Gain or loss percentage (EN)<br/>
    /// Thuộc tính tính toán: Phần trăm lãi hoặc lỗ (VI)
    /// </summary>
    [NotMapped]
    public decimal GainLossPercentage => InitialAmount > 0 ? (GainLoss / InitialAmount) * 100 : 0;

    /// <summary>
    /// Calculated property: Days held (EN)<br/>
    /// Thuộc tính tính toán: Số ngày nắm giữ (VI)
    /// </summary>
    [NotMapped]
    public int DaysHeld => (SaleDate ?? DateTime.UtcNow).Subtract(PurchaseDate).Days;

    /// <summary>
    /// Calculated property: Annualized return (EN)<br/>
    /// Thuộc tính tính toán: Lợi nhuận hàng năm (VI)
    /// </summary>
    [NotMapped]
    public decimal? AnnualizedReturn
    {
        get
        {
            if (DaysHeld <= 0 || InitialAmount <= 0) return null;
            var years = DaysHeld / 365.0m;
            if (years <= 0) return null;
            return (decimal)(Math.Pow((double)(CurrentValue / InitialAmount), (double)(1 / years)) - 1) * 100;
        }
    }
}
