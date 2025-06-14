namespace PlanningInvestment.Domain.Enums;

/// <summary>
/// Represents the category of a financial goal. (EN)<br/>
/// Biểu thị danh mục của một mục tiêu tài chính. (VI)
/// </summary>
public enum GoalCategory
{
    /// <summary>
    /// Emergency fund. (EN)<br/>
    /// Quỹ khẩn cấp. (VI)
    /// </summary>
    EmergencyFund = 0,

    /// <summary>
    /// House purchase. (EN)<br/>
    /// Mua nhà. (VI)
    /// </summary>
    House = 1,

    /// <summary>
    /// Car purchase. (EN)<br/>
    /// Mua xe. (VI)
    /// </summary>
    Car = 2,

    /// <summary>
    /// Education. (EN)<br/>
    /// Học tập. (VI)
    /// </summary>
    Education = 3,

    /// <summary>
    /// Retirement. (EN)<br/>
    /// Nghỉ hưu. (VI)
    /// </summary>
    Retirement = 4,

    /// <summary>
    /// Vacation. (EN)<br/>
    /// Du lịch. (VI)
    /// </summary>
    Vacation = 5,

    /// <summary>
    /// Investment. (EN)<br/>
    /// Đầu tư. (VI)
    /// </summary>
    Investment = 6,

    /// <summary>
    /// Wedding. (EN)<br/>
    /// Đám cưới. (VI)
    /// </summary>
    Wedding = 7,

    /// <summary>
    /// Business. (EN)<br/>
    /// Kinh doanh. (VI)
    /// </summary>
    Business = 8,

    /// <summary>
    /// Other. (EN)<br/>
    /// Khác. (VI)
    /// </summary>
    Other = 9
}
