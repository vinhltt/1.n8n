namespace PlanningInvestment.Domain.Enums;

/// <summary>
/// Represents the status of a debt. (EN)<br/>
/// Biểu thị trạng thái của một khoản nợ. (VI)
/// </summary>
public enum DebtStatus
{
    /// <summary>
    /// Debt is active. (EN)<br/>
    /// Khoản nợ đang hoạt động. (VI)
    /// </summary>
    Active = 0,

    /// <summary>
    /// Debt has been paid off. (EN)<br/>
    /// Khoản nợ đã được trả hết. (VI)
    /// </summary>
    PaidOff = 1,

    /// <summary>
    /// Debt is in default. (EN)<br/>
    /// Khoản nợ bị vỡ nợ. (VI)
    /// </summary>
    Defaulted = 2,

    /// <summary>
    /// Debt has been refinanced. (EN)<br/>
    /// Khoản nợ đã được tái cấu trúc. (VI)
    /// </summary>
    Refinanced = 3
}
