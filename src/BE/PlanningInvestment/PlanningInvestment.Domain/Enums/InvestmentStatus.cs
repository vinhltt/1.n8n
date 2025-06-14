namespace PlanningInvestment.Domain.Enums;

/// <summary>
/// Represents the status of an investment. (EN)<br/>
/// Biểu thị trạng thái của một khoản đầu tư. (VI)
/// </summary>
public enum InvestmentStatus
{
    /// <summary>
    /// Investment is active. (EN)<br/>
    /// Khoản đầu tư đang hoạt động. (VI)
    /// </summary>
    Active = 0,

    /// <summary>
    /// Investment has been sold. (EN)<br/>
    /// Khoản đầu tư đã được bán. (VI)
    /// </summary>
    Sold = 1,

    /// <summary>
    /// Investment has matured. (EN)<br/>
    /// Khoản đầu tư đã đến hạn. (VI)
    /// </summary>
    Matured = 2,

    /// <summary>
    /// Investment is suspended. (EN)<br/>
    /// Khoản đầu tư bị tạm ngừng. (VI)
    /// </summary>
    Suspended = 3
}
