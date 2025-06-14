namespace PlanningInvestment.Contracts.DTOs;

/// <summary>
/// Represents pagination information. (EN)<br/>
/// Đại diện cho thông tin phân trang. (VI)
/// </summary>
public class Pagination
{
    /// <summary>
    /// Gets or sets the current page number (1-based). (EN)<br/>
    /// Lấy hoặc đặt số trang hiện tại (bắt đầu từ 1). (VI)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page. (EN)<br/>
    /// Lấy hoặc đặt số mục trên mỗi trang. (VI)
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the total number of items. (EN)<br/>
    /// Lấy hoặc đặt tổng số mục. (VI)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the total number of pages. (EN)<br/>
    /// Lấy tổng số trang. (VI)
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets whether there is a previous page. (EN)<br/>
    /// Lấy thông tin có trang trước hay không. (VI)
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Gets whether there is a next page. (EN)<br/>
    /// Lấy thông tin có trang tiếp theo hay không. (VI)
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;
}

/// <summary>
/// Represents base pagination information for a collection of data. (EN)<br/>
/// Đại diện cho thông tin phân trang cơ sở cho một tập hợp dữ liệu. (VI)
/// </summary>
/// <typeparam name="T">The type of data in the collection.</typeparam>
public class BasePaging<T> : IBasePaging<T>
{
    /// <summary>
    /// Gets or sets the pagination details. (EN)<br/>
    /// Lấy hoặc đặt chi tiết phân trang. (VI)
    /// </summary>
    public Pagination Pagination { get; set; } = new();
    /// <summary>
    /// Gets or sets the collection of data for the current page. (EN)<br/>
    /// Lấy hoặc đặt tập hợp dữ liệu cho trang hiện tại. (VI)
    /// </summary>
    public IEnumerable<T>? Data { get; set; }
}

/// <summary>
/// Defines the interface for base pagination information. (EN)<br/>
/// Định nghĩa interface cho thông tin phân trang cơ sở. (VI)
/// </summary>
/// <typeparam name="T">The type of data in the collection.</typeparam>
public interface IBasePaging<T>
{
    /// <summary>
    /// Gets or sets the pagination details. (EN)<br/>
    /// Lấy hoặc đặt chi tiết phân trang. (VI)
    /// </summary>
    public Pagination Pagination { get; set; }
    /// <summary>
    /// Gets or sets the collection of data for the current page. (EN)<br/>
    /// Lấy hoặc đặt tập hợp dữ liệu cho trang hiện tại. (VI)
    /// </summary>
    public IEnumerable<T>? Data { get; set; }
}