namespace PlanningInvestment.Domain.Enums;

/// <summary>
/// Represents the type of goal transaction. (EN)<br/>
/// Biểu thị loại giao dịch mục tiêu. (VI)
/// </summary>
public enum GoalTransactionType
{
    /// <summary>
    /// Deposit money to goal. (EN)<br/>
    /// Nạp tiền vào mục tiêu. (VI)
    /// </summary>
    Deposit = 0,

    /// <summary>
    /// Withdraw money from goal. (EN)<br/>
    /// Rút tiền từ mục tiêu. (VI)
    /// </summary>
    Withdrawal = 1,

    /// <summary>
    /// Interest earned. (EN)<br/>
    /// Lãi suất kiếm được. (VI)
    /// </summary>
    Interest = 2,

    /// <summary>
    /// Bonus or gift. (EN)<br/>
    /// Thưởng hoặc quà tặng. (VI)
    /// </summary>
    Bonus = 3,

    /// <summary>
    /// Penalty or fee. (EN)<br/>
    /// Phạt hoặc phí. (VI)
    /// </summary>
    Penalty = 4
}
