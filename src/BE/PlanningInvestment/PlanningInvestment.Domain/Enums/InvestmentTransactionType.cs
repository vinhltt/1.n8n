namespace PlanningInvestment.Domain.Enums;

/// <summary>
/// Represents the type of investment transaction. (EN)<br/>
/// Biểu thị loại giao dịch đầu tư. (VI)
/// </summary>
public enum InvestmentTransactionType
{
    /// <summary>
    /// Buy investment. (EN)<br/>
    /// Mua đầu tư. (VI)
    /// </summary>
    Buy = 0,

    /// <summary>
    /// Sell investment. (EN)<br/>
    /// Bán đầu tư. (VI)
    /// </summary>
    Sell = 1,

    /// <summary>
    /// Dividend received. (EN)<br/>
    /// Cổ tức nhận được. (VI)
    /// </summary>
    Dividend = 2,

    /// <summary>
    /// Interest received. (EN)<br/>
    /// Lãi suất nhận được. (VI)
    /// </summary>
    Interest = 3,

    /// <summary>
    /// Stock split. (EN)<br/>
    /// Tách cổ phiếu. (VI)
    /// </summary>
    Split = 4,

    /// <summary>
    /// Bonus shares. (EN)<br/>
    /// Cổ phiếu thưởng. (VI)
    /// </summary>
    Bonus = 5,

    /// <summary>
    /// Transaction fee. (EN)<br/>
    /// Phí giao dịch. (VI)
    /// </summary>
    Fee = 6,

    /// <summary>
    /// Tax payment. (EN)<br/>
    /// Thanh toán thuế. (VI)
    /// </summary>
    Tax = 7
}
