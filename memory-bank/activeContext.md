# activeContext.md

## Trọng tâm công việc hiện tại
- **Đã hoàn thành triển khai tính năng Quản lý Giao dịch Định kỳ (Recurring Transactions) trong Core Finance bounded context.**
- **Đang hoàn thiện và kiểm thử các chức năng của RecurringTransactionTemplateService và ExpectedTransactionService.**
- Đảm bảo Memory Bank phản ánh đầy đủ nghiệp vụ, kiến trúc, yêu cầu chức năng/phi chức năng, tiến độ, insight mới nhất.
- **Đã cấu hình Dependency Injection cho các service và repository mới.**
- **Đã tạo và đăng ký các validator cho các request DTOs sử dụng FluentValidation.**
- **Chuẩn bị triển khai background job để tự động sinh giao dịch dự kiến từ các mẫu định kỳ.**

## Thay đổi gần đây
- **Đã triển khai đầy đủ tính năng Quản lý Giao dịch Định kỳ trong Core Finance (thay vì Planning & Investment như dự định ban đầu):**
  - **Tạo domain models: RecurringTransactionTemplate và ExpectedTransaction với đầy đủ properties và enums.**
  - **Triển khai RecurringTransactionTemplateService với các chức năng: tạo/sửa/xóa mẫu, sinh giao dịch dự kiến, tính toán ngày thực hiện tiếp theo.**
  - **Triển khai ExpectedTransactionService với các chức năng: quản lý giao dịch dự kiến, xác nhận/hủy/điều chỉnh, dự báo dòng tiền.**
  - **Tạo đầy đủ DTOs cho cả hai services: CreateRequest, UpdateRequest, ViewModel và các request đặc biệt.**
- **Đã tổ chức lại cấu trúc unit tests theo pattern mới:**
  - **Sử dụng partial class cho mỗi service test (AccountServiceTests, ExpectedTransactionServiceTests, RecurringTransactionTemplateServiceTests).**
  - **Mỗi method của service có file test riêng theo format ServiceNameTests.MethodName.cs.**
  - **Tất cả test files được tổ chức trong thư mục con theo tên service.**
  - **Di chuyển helper functions vào TestHelpers.cs trong thư mục Helpers.**
- **Đã viết comprehensive unit tests cho tất cả methods của cả hai services mới.**
- **Đã cấu hình Dependency Injection cho Unit of Work, RecurringTransactionTemplateService và ExpectedTransactionService trong ServiceExtensions.**
- **Đã tạo các validator bằng FluentValidation cho các request DTOs liên quan đến RecurringTransactionTemplate và ExpectedTransaction, và đăng ký chúng tập trung bằng extension method AddApplicationValidators.**
- **Đã cập nhật DataAnnotations cho tất cả Entity trong CoreFinance.Domain:**
  - **Thêm [Column(TypeName = "decimal(18,2)")] cho các property decimal liên quan đến tiền**
  - **Thêm [Range] validation cho các giá trị số**
  - **Thêm [MaxLength] cho các string properties**
  - **Thêm [Required] cho các property bắt buộc**
- **Đã thay đổi Foreign Key design pattern: Chuyển tất cả Foreign Keys từ Guid sang Guid? (nullable) để tạo mối quan hệ linh hoạt hơn:**
  - **Transaction: AccountId, UserId → Guid?**
  - **RecurringTransactionTemplate: UserId, AccountId → Guid?**
  - **ExpectedTransaction: RecurringTransactionTemplateId, UserId, AccountId → Guid?**
  - **Mục đích: Tăng tính linh hoạt, hỗ trợ import dữ liệu không đầy đủ, soft delete, orphaned records management**

## Bước tiếp theo
- **Triển khai background job service (sử dụng Quartz.NET hoặc Hangfire) để tự động sinh giao dịch dự kiến.**
- **Tích hợp với NotificationService để gửi thông báo về giao dịch định kỳ sắp đến hạn.**
- **Cập nhật ReportingService để tạo báo cáo kế hoạch tiền mặt kết hợp giao dịch thực tế và dự kiến.**
- **Triển khai API Controllers cho RecurringTransactionTemplate và ExpectedTransaction.**
- **Thiết kế và triển khai giao diện người dùng cho việc quản lý mẫu giao dịch định kỳ.**
- Bổ sung chi tiết user stories, acceptance criteria cho từng chức năng còn lại.
- Làm việc với đội UI/UX để hoàn thiện mockup, wireframe.
- Xác định giải pháp frontend cụ thể.

## Quyết định quan trọng
- **Quyết định triển khai tính năng Recurring Transactions trong Core Finance thay vì Planning & Investment để tận dụng cơ sở hạ tầng sẵn có và đơn giản hóa việc tích hợp.**
- **Sử dụng enum RecurrenceFrequency với các giá trị cố định (Daily=1, Weekly=7, Monthly=30, etc.) và hỗ trợ Custom frequency với CustomIntervalDays.**
- **Thiết kế ExpectedTransaction với lifecycle đầy đủ: Pending → Confirmed/Cancelled/Completed.**
- **Hỗ trợ điều chỉnh giao dịch dự kiến (adjustment) với lưu trữ OriginalAmount và AdjustmentReason.**
- **Quyết định sử dụng nullable Foreign Keys (Guid?) thay vì non-nullable (Guid) để tạo mối quan hệ linh hoạt hơn giữa các Entity.**
- Ưu tiên triển khai core service (Identity, Core Finance, Reporting) trước.
- Import statement: bắt đầu với manual upload, lên kế hoạch tích hợp API ngân hàng/aggregator sau.
- Đảm bảo mọi service có health check, logging, monitoring, alerting tự động.
- Định nghĩa rõ acceptance criteria, test coverage >80%.

## Pattern, insight, lưu ý
- **Đã áp dụng thành công pattern tổ chức unit test với partial class, giúp code dễ maintain và tìm kiếm.**
- **RecurringTransactionTemplateService có khả năng sinh giao dịch dự kiến theo batch (GenerateExpectedTransactionsForAllActiveTemplatesAsync) phù hợp cho background job.**
- **ExpectedTransactionService cung cấp các method dự báo dòng tiền (GetCashFlowForecastAsync, GetCategoryForecastAsync) hỗ trợ báo cáo tài chính.**
- **Thiết kế domain models với đầy đủ navigation properties hỗ trợ Entity Framework relationships.**
- **Foreign Key Design Pattern với nullable Guid cho phép:**
  - **Tạo Entity mà không cần liên kết ngay lập tức**
  - **Xử lý dữ liệu import không đầy đủ từ nguồn bên ngoài**
  - **Hỗ trợ soft delete và quản lý orphaned records**
  - **Tăng tính linh hoạt trong thiết kế API và business logic**
- Mọi thay đổi lớn về nghiệp vụ, kiến trúc, công nghệ đều phải cập nhật vào Memory Bank.
- Đảm bảo các yêu cầu phi chức năng (NFR) được kiểm thử và giám sát liên tục.
- Luôn review lại BA Design và Memory Bank trước khi bắt đầu task mới.
- Đã chuẩn hóa sử dụng Bogus cho fake data trong unit test, tuân thủ .NET rule.
- Checklist phát triển/test phải có hướng dẫn sử dụng Bogus cho data giả lập.
- **Chỉ sử dụng xUnit cho unit test, không dùng NUnit hay framework khác.**
- **Chuẩn hóa sử dụng FluentAssertions cho assert kết quả trong unit test, tuân thủ .NET rule.**
- **Quy ước sử dụng instance AutoMapper thật (không mock) cho unit test ở tầng service, dựa vào các AutoMapper profile đã được cấu hình đúng và đã được test riêng.**
- **Đã chuẩn hóa việc đăng ký validator bằng extension method AddApplicationValidators để dễ quản lý.**
- **Lưu ý về việc đồng bộ dữ liệu giữa giao dịch dự kiến (ExpectedTransaction) và giao dịch thực tế (Transaction) thông qua ActualTransactionId khi confirm expected transaction.** 