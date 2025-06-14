namespace PlanningInvestment.Domain.Enums;

/// <summary>
/// Represents the status of a debt payment. (EN)<br/>
/// Biểu thị trạng thái của một khoản thanh toán nợ. (VI)
/// </summary>
public enum DebtPaymentStatus
{
    /// <summary>
    /// Payment completed. (EN)<br/>
    /// Thanh toán hoàn thành. (VI)
    /// </summary>
    Completed = 0,

    /// <summary>
    /// Payment is pending. (EN)<br/>
    /// Thanh toán đang chờ. (VI)
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Payment failed. (EN)<br/>
    /// Thanh toán thất bại. (VI)
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Payment cancelled. (EN)<br/>
    /// Thanh toán bị hủy. (VI)
    /// </summary>
    Cancelled = 3
}
