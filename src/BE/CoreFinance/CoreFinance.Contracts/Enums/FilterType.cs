namespace CoreFinance.Contracts.Enums;

/// <summary>
/// Defines the types of filters that can be applied. (EN)
/// <br/>
/// Định nghĩa các loại bộ lọc có thể áp dụng. (VI)
/// </summary>
public enum FilterType
{
    /// <summary>
    /// Equal to the specified value. (EN)
    /// <br/>
    /// Bằng giá trị được chỉ định. (VI)
    /// </summary>
    Equal,
    /// <summary>
    /// Not equal to the specified value. (EN)
    /// <br/>
    /// Không bằng giá trị được chỉ định. (VI)
    /// </summary>
    NotEqual,
    /// <summary>
    /// Starts with the specified string. (EN)
    /// <br/>
    /// Bắt đầu bằng chuỗi được chỉ định. (VI)
    /// </summary>
    StartsWith,
    /// <summary>
    /// Ends with the specified string. (EN)
    /// <br/>
    /// Kết thúc bằng chuỗi được chỉ định. (VI)
    /// </summary>
    EndsWith,
    /// <summary>
    /// Greater than the specified value. (EN)
    /// <br/>
    /// Lớn hơn giá trị được chỉ định. (VI)
    /// </summary>
    GreaterThan,
    /// <summary>
    /// Less than the specified value. (EN)
    /// <br/>
    /// Nhỏ hơn giá trị được chỉ định. (VI)
    /// </summary>
    LessThan,
    /// <summary>
    /// Less than or equal to the specified value. (EN)
    /// <br/>
    /// Nhỏ hơn hoặc bằng giá trị được chỉ định. (VI)
    /// </summary>
    LessThanOrEqual,
    /// <summary>
    /// Greater than or equal to the specified value. (EN)
    /// <br/>
    /// Lớn hơn hoặc bằng giá trị được chỉ định. (VI)
    /// </summary>
    GreaterThanOrEqual,
    /// <summary>
    /// Between the specified values. (EN)
    /// <br/>
    /// Nằm giữa các giá trị được chỉ định. (VI)
    /// </summary>
    Between,
    /// <summary>
    /// Not between the specified values. (EN)
    /// <br/>
    /// Không nằm giữa các giá trị được chỉ định. (VI)
    /// </summary>
    NotBetween,
    /// <summary>
    /// Is not null. (EN)
    /// <br/>
    /// Không phải null. (VI)
    /// </summary>
    IsNotNull,
    /// <summary>
    /// Is null. (EN)
    /// <br/>
    /// Là null. (VI)
    /// </summary>
    IsNull,
    /// <summary>
    /// Is not null or white space. (EN)
    /// <br/>
    /// Không phải null hoặc khoảng trắng. (VI)
    /// </summary>
    IsNotNullOrWhiteSpace,
    /// <summary>
    /// Is null or white space. (EN)
    /// <br/>
    /// Là null hoặc khoảng trắng. (VI)
    /// </summary>
    IsNullOrWhiteSpace,
    /// <summary>
    /// Is empty string. (EN)
    /// <br/>
    /// Là chuỗi rỗng. (VI)
    /// </summary>
    IsEmpty,
    /// <summary>
    /// Is not empty string. (EN)
    /// <br/>
    /// Không phải chuỗi rỗng. (VI)
    /// </summary>
    IsNotEmpty,
    /// <summary>
    /// Is in the specified list of values. (EN)
    /// <br/>
    /// Nằm trong danh sách các giá trị được chỉ định. (VI)
    /// </summary>
    In,
    /// <summary>
    /// Is not in the specified list of values. (EN)
    /// <br/>
    /// Không nằm trong danh sách các giá trị được chỉ định. (VI)
    /// </summary>
    NotIn,
    /// <summary>
    /// Contains the specified substring. (EN)
    /// <br/>
    /// Chứa chuỗi con được chỉ định. (VI)
    /// </summary>
    Contains,
    /// <summary>
    /// Does not contain the specified substring. (EN)
    /// <br/>
    /// Không chứa chuỗi con được chỉ định. (VI)
    /// </summary>
    NotContains
}