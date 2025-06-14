# PlanningInvestment Bounded Context - Detailed Design

## Overview
PlanningInvestment bounded context quản lý việc lập kế hoạch tài chính dài hạn bao gồm quản lý nợ, đặt mục tiêu tài chính và theo dõi đầu tư.

## Domain Models

### 1. Goal Entity (Mục tiêu tài chính)
```csharp
public class Goal : BaseEntity<Guid>
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } // Tên mục tiêu (EN) / Tên mục tiêu (VI)

    [MaxLength(1000)]
    public string? Description { get; set; } // Mô tả chi tiết (EN) / Mô tả chi tiết (VI)

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99)]
    public decimal TargetAmount { get; set; } // Số tiền mục tiêu (EN) / Số tiền mục tiêu (VI)

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99)]
    public decimal CurrentAmount { get; set; } = 0; // Số tiền hiện tại (EN) / Số tiền hiện tại (VI)

    [Required]
    public DateTime StartDate { get; set; } // Ngày bắt đầu (EN) / Ngày bắt đầu (VI)

    public DateTime? TargetDate { get; set; } // Ngày mục tiêu (EN) / Ngày mục tiêu (VI)

    [Required]
    public GoalStatus Status { get; set; } = GoalStatus.Planning; // Trạng thái mục tiêu (EN) / Trạng thái mục tiêu (VI)

    [Required]
    public GoalCategory Category { get; set; } // Danh mục mục tiêu (EN) / Danh mục mục tiêu (VI)

    [Range(0, 100)]
    public decimal Priority { get; set; } = 50; // Độ ưu tiên (0-100) (EN) / Độ ưu tiên (0-100) (VI)

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99)]
    public decimal MonthlyContribution { get; set; } = 0; // Đóng góp hàng tháng (EN) / Đóng góp hàng tháng (VI)

    // Audit fields from BaseEntity
    public Guid? UserId { get; set; } // Foreign key nullable (EN) / Khóa ngoại nullable (VI)

    // Navigation properties
    public virtual ICollection<GoalTransaction> GoalTransactions { get; set; } = new List<GoalTransaction>();
}
```

### 2. Investment Entity (Khoản đầu tư)
```csharp
public class Investment : BaseEntity<Guid>
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } // Tên khoản đầu tư (EN) / Tên khoản đầu tư (VI)

    [MaxLength(1000)]
    public string? Description { get; set; } // Mô tả chi tiết (EN) / Mô tả chi tiết (VI)

    [Required]
    public InvestmentType Type { get; set; } // Loại đầu tư (EN) / Loại đầu tư (VI)

    [MaxLength(20)]
    public string? Symbol { get; set; } // Mã chứng khoán (EN) / Mã chứng khoán (VI)

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99)]
    public decimal InitialAmount { get; set; } // Số tiền đầu tư ban đầu (EN) / Số tiền đầu tư ban đầu (VI)

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99)]
    public decimal CurrentValue { get; set; } // Giá trị hiện tại (EN) / Giá trị hiện tại (VI)

    [Required]
    public DateTime PurchaseDate { get; set; } // Ngày mua (EN) / Ngày mua (VI)

    public DateTime? SaleDate { get; set; } // Ngày bán (EN) / Ngày bán (VI)

    [Column(TypeName = "decimal(5,2)")]
    [Range(-100, 1000)]
    public decimal? ReturnRate { get; set; } // Tỷ suất sinh lời (%) (EN) / Tỷ suất sinh lời (%) (VI)

    [Required]
    public InvestmentStatus Status { get; set; } = InvestmentStatus.Active; // Trạng thái đầu tư (EN) / Trạng thái đầu tư (VI)

    [Range(1, 10)]
    public int RiskLevel { get; set; } = 5; // Mức độ rủi ro (1-10) (EN) / Mức độ rủi ro (1-10) (VI)

    [MaxLength(100)]
    public string? Platform { get; set; } // Sàn giao dịch (EN) / Sàn giao dịch (VI)

    // Audit fields from BaseEntity
    public Guid? UserId { get; set; } // Foreign key nullable (EN) / Khóa ngoại nullable (VI)

    // Navigation properties
    public virtual ICollection<InvestmentTransaction> InvestmentTransactions { get; set; } = new List<InvestmentTransaction>();
}
```

### 3. Enhanced Debt Entity (Khoản nợ - đã có sẵn, cần kiểm tra)
```csharp
public class Debt : BaseEntity<Guid>
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } // Tên khoản nợ (EN) / Tên khoản nợ (VI)

    [MaxLength(1000)]
    public string? Description { get; set; } // Mô tả chi tiết (EN) / Mô tả chi tiết (VI)

    [Required]
    public DebtType Type { get; set; } // Loại nợ (EN) / Loại nợ (VI)

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99)]
    public decimal OriginalAmount { get; set; } // Số tiền nợ ban đầu (EN) / Số tiền nợ ban đầu (VI)

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99)]
    public decimal RemainingAmount { get; set; } // Số tiền nợ còn lại (EN) / Số tiền nợ còn lại (VI)

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 100)]
    public decimal InterestRate { get; set; } // Lãi suất (%) (EN) / Lãi suất (%) (VI)

    [Required]
    public DateTime StartDate { get; set; } // Ngày bắt đầu (EN) / Ngày bắt đầu (VI)

    public DateTime? DueDate { get; set; } // Ngày đến hạn (EN) / Ngày đến hạn (VI)

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99)]
    public decimal? MonthlyPayment { get; set; } // Thanh toán hàng tháng (EN) / Thanh toán hàng tháng (VI)

    [Required]
    public DebtStatus Status { get; set; } = DebtStatus.Active; // Trạng thái nợ (EN) / Trạng thái nợ (VI)

    [MaxLength(200)]
    public string? Creditor { get; set; } // Chủ nợ (EN) / Chủ nợ (VI)

    // Audit fields from BaseEntity
    public Guid? UserId { get; set; } // Foreign key nullable (EN) / Khóa ngoại nullable (VI)

    // Navigation properties
    public virtual ICollection<DebtPayment> DebtPayments { get; set; } = new List<DebtPayment>();
}
```

### 4. Supporting Entities

#### GoalTransaction Entity
```csharp
public class GoalTransaction : BaseEntity<Guid>
{
    [Required]
    public Guid GoalId { get; set; } // ID mục tiêu (EN) / ID mục tiêu (VI)

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; } // Số tiền (EN) / Số tiền (VI)

    [Required]
    public DateTime TransactionDate { get; set; } // Ngày giao dịch (EN) / Ngày giao dịch (VI)

    [Required]
    public GoalTransactionType Type { get; set; } // Loại giao dịch (EN) / Loại giao dịch (VI)

    [MaxLength(500)]
    public string? Description { get; set; } // Mô tả (EN) / Mô tả (VI)

    // Navigation properties
    public virtual Goal Goal { get; set; } = null!;
}
```

#### InvestmentTransaction Entity
```csharp
public class InvestmentTransaction : BaseEntity<Guid>
{
    [Required]
    public Guid InvestmentId { get; set; } // ID đầu tư (EN) / ID đầu tư (VI)

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; } // Số tiền (EN) / Số tiền (VI)

    [Required]
    public DateTime TransactionDate { get; set; } // Ngày giao dịch (EN) / Ngày giao dịch (VI)

    [Required]
    public InvestmentTransactionType Type { get; set; } // Loại giao dịch (EN) / Loại giao dịch (VI)

    [Column(TypeName = "decimal(18,4)")]
    [Range(0, 999999999.9999)]
    public decimal? Quantity { get; set; } // Số lượng (EN) / Số lượng (VI)

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99)]
    public decimal? UnitPrice { get; set; } // Giá đơn vị (EN) / Giá đơn vị (VI)

    [MaxLength(500)]
    public string? Description { get; set; } // Mô tả (EN) / Mô tả (VI)

    // Navigation properties
    public virtual Investment Investment { get; set; } = null!;
}
```

#### DebtPayment Entity
```csharp
public class DebtPayment : BaseEntity<Guid>
{
    [Required]
    public Guid DebtId { get; set; } // ID khoản nợ (EN) / ID khoản nợ (VI)

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; } // Số tiền thanh toán (EN) / Số tiền thanh toán (VI)

    [Required]
    public DateTime PaymentDate { get; set; } // Ngày thanh toán (EN) / Ngày thanh toán (VI)

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99)]
    public decimal? PrincipalAmount { get; set; } // Số tiền gốc (EN) / Số tiền gốc (VI)

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999999.99)]
    public decimal? InterestAmount { get; set; } // Số tiền lãi (EN) / Số tiền lãi (VI)

    [MaxLength(500)]
    public string? Description { get; set; } // Mô tả (EN) / Mô tả (VI)

    [Required]
    public DebtPaymentStatus Status { get; set; } = DebtPaymentStatus.Completed; // Trạng thái thanh toán (EN) / Trạng thái thanh toán (VI)

    // Navigation properties
    public virtual Debt Debt { get; set; } = null!;
}
```

## Enums

### Goal Related Enums
```csharp
public enum GoalStatus
{
    Planning = 0,     // Đang lập kế hoạch (EN) / Đang lập kế hoạch (VI)
    Active = 1,       // Đang thực hiện (EN) / Đang thực hiện (VI)
    Paused = 2,       // Tạm dừng (EN) / Tạm dừng (VI)
    Completed = 3,    // Hoàn thành (EN) / Hoàn thành (VI)
    Cancelled = 4     // Hủy bỏ (EN) / Hủy bỏ (VI)
}

public enum GoalCategory
{
    EmergencyFund = 0,    // Quỹ khẩn cấp (EN) / Quỹ khẩn cấp (VI)
    House = 1,            // Mua nhà (EN) / Mua nhà (VI)
    Car = 2,              // Mua xe (EN) / Mua xe (VI)
    Education = 3,        // Học tập (EN) / Học tập (VI)
    Retirement = 4,       // Nghỉ hưu (EN) / Nghỉ hưu (VI)
    Vacation = 5,         // Du lịch (EN) / Du lịch (VI)
    Investment = 6,       // Đầu tư (EN) / Đầu tư (VI)
    Wedding = 7,          // Đám cưới (EN) / Đám cưới (VI)
    Business = 8,         // Kinh doanh (EN) / Kinh doanh (VI)
    Other = 9             // Khác (EN) / Khác (VI)
}

public enum GoalTransactionType
{
    Deposit = 0,      // Nạp tiền (EN) / Nạp tiền (VI)
    Withdrawal = 1,   // Rút tiền (EN) / Rút tiền (VI)
    Interest = 2,     // Lãi suất (EN) / Lãi suất (VI)
    Bonus = 3,        // Thưởng (EN) / Thưởng (VI)
    Penalty = 4       // Phạt (EN) / Phạt (VI)
}
```

### Investment Related Enums
```csharp
public enum InvestmentType
{
    Stock = 0,            // Cổ phiếu (EN) / Cổ phiếu (VI)
    Bond = 1,             // Trái phiếu (EN) / Trái phiếu (VI)
    MutualFund = 2,       // Quỹ tương hỗ (EN) / Quỹ tương hỗ (VI)
    ETF = 3,              // Quỹ ETF (EN) / Quỹ ETF (VI)
    RealEstate = 4,       // Bất động sản (EN) / Bất động sản (VI)
    Cryptocurrency = 5,   // Tiền mã hóa (EN) / Tiền mã hóa (VI)
    Commodity = 6,        // Hàng hóa (EN) / Hàng hóa (VI)
    FixedDeposit = 7,     // Tiền gửi có kỳ hạn (EN) / Tiền gửi có kỳ hạn (VI)
    Gold = 8,             // Vàng (EN) / Vàng (VI)
    Other = 9             // Khác (EN) / Khác (VI)
}

public enum InvestmentStatus
{
    Active = 0,       // Đang hoạt động (EN) / Đang hoạt động (VI)
    Sold = 1,         // Đã bán (EN) / Đã bán (VI)
    Matured = 2,      // Đã đến hạn (EN) / Đã đến hạn (VI)
    Suspended = 3     // Tạm ngừng (EN) / Tạm ngừng (VI)
}

public enum InvestmentTransactionType
{
    Buy = 0,              // Mua (EN) / Mua (VI)
    Sell = 1,             // Bán (EN) / Bán (VI)
    Dividend = 2,         // Cổ tức (EN) / Cổ tức (VI)
    Interest = 3,         // Lãi suất (EN) / Lãi suất (VI)
    Split = 4,            // Tách cổ phiếu (EN) / Tách cổ phiếu (VI)
    Bonus = 5,            // Cổ phiếu thưởng (EN) / Cổ phiếu thưởng (VI)
    Fee = 6,              // Phí (EN) / Phí (VI)
    Tax = 7               // Thuế (EN) / Thuế (VI)
}
```

### Debt Related Enums
```csharp
public enum DebtType
{
    CreditCard = 0,       // Thẻ tín dụng (EN) / Thẻ tín dụng (VI)
    PersonalLoan = 1,     // Vay cá nhân (EN) / Vay cá nhân (VI)
    Mortgage = 2,         // Vay mua nhà (EN) / Vay mua nhà (VI)
    CarLoan = 3,          // Vay mua xe (EN) / Vay mua xe (VI)
    StudentLoan = 4,      // Vay học tập (EN) / Vay học tập (VI)
    BusinessLoan = 5,     // Vay kinh doanh (EN) / Vay kinh doanh (VI)
    FamilyLoan = 6,       // Vay gia đình (EN) / Vay gia đình (VI)
    Other = 7             // Khác (EN) / Khác (VI)
}

public enum DebtStatus
{
    Active = 0,       // Đang hoạt động (EN) / Đang hoạt động (VI)
    PaidOff = 1,      // Đã trả hết (EN) / Đã trả hết (VI)
    Defaulted = 2,    // Vỡ nợ (EN) / Vỡ nợ (VI)
    Refinanced = 3    // Tái cấu trúc (EN) / Tái cấu trúc (VI)
}

public enum DebtPaymentStatus
{
    Completed = 0,    // Hoàn thành (EN) / Hoàn thành (VI)
    Pending = 1,      // Đang chờ (EN) / Đang chờ (VI)
    Failed = 2,       // Thất bại (EN) / Thất bại (VI)
    Cancelled = 3     // Hủy bỏ (EN) / Hủy bỏ (VI)
}
```

## Service Interfaces

### IGoalService
```csharp
public interface IGoalService
{
    // Basic CRUD Operations
    Task<GoalViewModel?> GetGoalByIdAsync(Guid id);
    Task<IEnumerable<GoalViewModel>> GetAllGoalsAsync();
    Task<IBasePaging<GoalViewModel>> GetGoalsPagingAsync(FilterBodyRequest request);
    Task<GoalViewModel> CreateGoalAsync(CreateGoalRequest request);
    Task<GoalViewModel> UpdateGoalAsync(Guid id, UpdateGoalRequest request);
    Task<bool> DeleteGoalAsync(Guid id);

    // Goal-specific Operations
    Task<bool> AddContributionAsync(Guid goalId, AddGoalContributionRequest request);
    Task<bool> WithdrawFromGoalAsync(Guid goalId, WithdrawFromGoalRequest request);
    Task<GoalProgressViewModel> GetGoalProgressAsync(Guid goalId);
    Task<IEnumerable<GoalTransactionViewModel>> GetGoalTransactionsAsync(Guid goalId);
    Task<GoalAnalyticsViewModel> GetGoalAnalyticsAsync(Guid goalId);
    Task<IEnumerable<GoalViewModel>> GetGoalsByStatusAsync(GoalStatus status);
    Task<IEnumerable<GoalViewModel>> GetGoalsByCategoryAsync(GoalCategory category);
}
```

### IInvestmentService
```csharp
public interface IInvestmentService
{
    // Basic CRUD Operations
    Task<InvestmentViewModel?> GetInvestmentByIdAsync(Guid id);
    Task<IEnumerable<InvestmentViewModel>> GetAllInvestmentsAsync();
    Task<IBasePaging<InvestmentViewModel>> GetInvestmentsPagingAsync(FilterBodyRequest request);
    Task<InvestmentViewModel> CreateInvestmentAsync(CreateInvestmentRequest request);
    Task<InvestmentViewModel> UpdateInvestmentAsync(Guid id, UpdateInvestmentRequest request);
    Task<bool> DeleteInvestmentAsync(Guid id);

    // Investment-specific Operations
    Task<bool> AddTransactionAsync(Guid investmentId, AddInvestmentTransactionRequest request);
    Task<InvestmentPerformanceViewModel> GetInvestmentPerformanceAsync(Guid investmentId);
    Task<IEnumerable<InvestmentTransactionViewModel>> GetInvestmentTransactionsAsync(Guid investmentId);
    Task<PortfolioSummaryViewModel> GetPortfolioSummaryAsync();
    Task<IEnumerable<InvestmentViewModel>> GetInvestmentsByTypeAsync(InvestmentType type);
    Task<bool> UpdateCurrentValueAsync(Guid investmentId, decimal currentValue);
    Task<InvestmentAnalyticsViewModel> GetInvestmentAnalyticsAsync();
}
```

### IDebtService
```csharp
public interface IDebtService
{
    // Basic CRUD Operations
    Task<DebtViewModel?> GetDebtByIdAsync(Guid id);
    Task<IEnumerable<DebtViewModel>> GetAllDebtsAsync();
    Task<IBasePaging<DebtViewModel>> GetDebtsPagingAsync(FilterBodyRequest request);
    Task<DebtViewModel> CreateDebtAsync(CreateDebtRequest request);
    Task<DebtViewModel> UpdateDebtAsync(Guid id, UpdateDebtRequest request);
    Task<bool> DeleteDebtAsync(Guid id);

    // Debt-specific Operations
    Task<bool> MakePaymentAsync(Guid debtId, MakeDebtPaymentRequest request);
    Task<IEnumerable<DebtPaymentViewModel>> GetDebtPaymentsAsync(Guid debtId);
    Task<DebtSummaryViewModel> GetDebtSummaryAsync();
    Task<IEnumerable<DebtViewModel>> GetDebtsByTypeAsync(DebtType type);
    Task<PaymentScheduleViewModel> GeneratePaymentScheduleAsync(Guid debtId);
    Task<DebtAnalyticsViewModel> GetDebtAnalyticsAsync();
    Task<bool> CalculateInterestAsync(Guid debtId);
}
```

## DTOs Structure

### Goal DTOs
```csharp
// View Models
public class GoalViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? TargetDate { get; set; }
    public GoalStatus Status { get; set; }
    public GoalCategory Category { get; set; }
    public decimal Priority { get; set; }
    public decimal MonthlyContribution { get; set; }
    public decimal ProgressPercentage { get; set; } // Calculated
    public int DaysRemaining { get; set; } // Calculated
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Request DTOs
public class CreateGoalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; } = 0;
    public DateTime StartDate { get; set; }
    public DateTime? TargetDate { get; set; }
    public GoalCategory Category { get; set; }
    public decimal Priority { get; set; } = 50;
    public decimal MonthlyContribution { get; set; } = 0;
}

public class UpdateGoalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public GoalStatus Status { get; set; }
    public decimal Priority { get; set; }
    public decimal MonthlyContribution { get; set; }
}
```

### Investment DTOs
```csharp
// View Models
public class InvestmentViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public InvestmentType Type { get; set; }
    public string? Symbol { get; set; }
    public decimal InitialAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? SaleDate { get; set; }
    public decimal? ReturnRate { get; set; }
    public InvestmentStatus Status { get; set; }
    public int RiskLevel { get; set; }
    public string? Platform { get; set; }
    public decimal GainLoss { get; set; } // Calculated
    public decimal GainLossPercentage { get; set; } // Calculated
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Request DTOs
public class CreateInvestmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public InvestmentType Type { get; set; }
    public string? Symbol { get; set; }
    public decimal InitialAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public DateTime PurchaseDate { get; set; }
    public int RiskLevel { get; set; } = 5;
    public string? Platform { get; set; }
}

public class UpdateInvestmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal CurrentValue { get; set; }
    public DateTime? SaleDate { get; set; }
    public InvestmentStatus Status { get; set; }
    public int RiskLevel { get; set; }
    public string? Platform { get; set; }
}
```

### Debt DTOs
```csharp
// View Models
public class DebtViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DebtType Type { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? MonthlyPayment { get; set; }
    public DebtStatus Status { get; set; }
    public string? Creditor { get; set; }
    public decimal PaidAmount { get; set; } // Calculated
    public decimal ProgressPercentage { get; set; } // Calculated
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Request DTOs
public class CreateDebtRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DebtType Type { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? MonthlyPayment { get; set; }
    public string? Creditor { get; set; }
}

public class UpdateDebtRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? MonthlyPayment { get; set; }
    public DebtStatus Status { get; set; }
    public string? Creditor { get; set; }
}
```

## API Endpoints

### Goal Controller
```
GET    /api/goals                    - Get all goals
GET    /api/goals/{id}               - Get goal by ID
POST   /api/goals                    - Create new goal
PUT    /api/goals/{id}               - Update goal
DELETE /api/goals/{id}               - Delete goal
POST   /api/goals/search             - Search goals with pagination
POST   /api/goals/{id}/contribute    - Add contribution to goal
POST   /api/goals/{id}/withdraw      - Withdraw from goal
GET    /api/goals/{id}/progress      - Get goal progress
GET    /api/goals/{id}/transactions  - Get goal transactions
GET    /api/goals/analytics          - Get goals analytics
```

### Investment Controller
```
GET    /api/investments                    - Get all investments
GET    /api/investments/{id}               - Get investment by ID
POST   /api/investments                    - Create new investment
PUT    /api/investments/{id}               - Update investment
DELETE /api/investments/{id}               - Delete investment
POST   /api/investments/search             - Search investments with pagination
POST   /api/investments/{id}/transactions  - Add investment transaction
GET    /api/investments/{id}/performance   - Get investment performance
GET    /api/investments/{id}/transactions  - Get investment transactions
GET    /api/investments/portfolio          - Get portfolio summary
PUT    /api/investments/{id}/value         - Update current value
GET    /api/investments/analytics          - Get investments analytics
```

### Debt Controller
```
GET    /api/debts                    - Get all debts
GET    /api/debts/{id}               - Get debt by ID
POST   /api/debts                    - Create new debt
PUT    /api/debts/{id}               - Update debt
DELETE /api/debts/{id}               - Delete debt
POST   /api/debts/search             - Search debts with pagination
POST   /api/debts/{id}/payments      - Make debt payment
GET    /api/debts/{id}/payments      - Get debt payments
GET    /api/debts/summary            - Get debt summary
GET    /api/debts/{id}/schedule      - Get payment schedule
GET    /api/debts/analytics          - Get debt analytics
POST   /api/debts/{id}/calculate-interest - Calculate interest
```

## Database Schema

### Tables
1. **Goals** - Mục tiêu tài chính
2. **GoalTransactions** - Giao dịch mục tiêu
3. **Investments** - Khoản đầu tư
4. **InvestmentTransactions** - Giao dịch đầu tư
5. **Debts** - Khoản nợ
6. **DebtPayments** - Thanh toán nợ

### Relationships
- Goal 1:N GoalTransactions
- Investment 1:N InvestmentTransactions
- Debt 1:N DebtPayments
- All entities có audit fields từ BaseEntity

## Implementation Plan

### Phase 1: Domain Layer
1. ✅ Kiểm tra Debt entity hiện có
2. 🔄 Tạo Goal và Investment entities
3. 🔄 Tạo supporting entities (GoalTransaction, InvestmentTransaction, DebtPayment)
4. 🔄 Tạo tất cả enums cần thiết

### Phase 2: Infrastructure Layer
1. 🔄 Tạo repositories cho tất cả entities
2. 🔄 Implement UnitOfWork pattern
3. 🔄 Tạo PlanningInvestmentDbContext
4. 🔄 Configure Entity Framework mappings

### Phase 3: Application Layer
1. 🔄 Implement GoalService với tất cả methods
2. 🔄 Implement InvestmentService với portfolio logic
3. 🔄 Implement DebtService với payment calculations
4. 🔄 Tạo tất cả DTOs và AutoMapper profiles
5. 🔄 Implement FluentValidation validators

### Phase 4: API Layer
1. 🔄 Tạo GoalController với RESTful endpoints
2. 🔄 Tạo InvestmentController với performance tracking
3. 🔄 Tạo DebtController với payment management
4. 🔄 Configure dependency injection
5. 🔄 Add Swagger documentation

### Phase 5: Testing & Documentation
1. 🔄 Unit tests cho tất cả services
2. 🔄 Integration tests cho API controllers
3. 🔄 Update Memory Bank documentation
4. 🔄 API documentation hoàn chỉnh

## Success Metrics
- ✅ All entities created và validated
- ✅ Services implement tất cả required methods
- ✅ API endpoints hoạt động đúng
- ✅ Unit test coverage > 80%
- ✅ Build success với 0 errors
- ✅ Integration với frontend ready
