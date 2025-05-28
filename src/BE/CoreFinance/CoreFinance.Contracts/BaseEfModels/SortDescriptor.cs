using CoreFinance.Contracts.Enums;

namespace CoreFinance.Contracts.BaseEfModels;

/// <summary>
/// Represents sorting criteria for data retrieval.<br/>(EN) Represents sorting criteria for data retrieval.<br/>(VI) Đại diện cho tiêu chí sắp xếp để truy xuất dữ liệu.
/// </summary>
public class SortDescriptor
{
    /// <summary>
    /// Gets or sets the field to sort by.<br/>(EN) Gets or sets the field to sort by.<br/>(VI) Lấy hoặc đặt trường để sắp xếp.
    /// </summary>
    public string? Field { get; set; }
    /// <summary>
    /// Gets or sets the sort direction (Ascending or Descending).<br/>(EN) Gets or sets the sort direction (Ascending or Descending).<br/>(VI) Lấy hoặc đặt hướng sắp xếp (Tăng dần hoặc Giảm dần).
    /// </summary>
    public SortDirection Direction { get; set; }
}