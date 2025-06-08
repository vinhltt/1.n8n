# systemPatterns.md

## Kiến trúc hệ thống
- Microservices kết hợp với workflow engine (n8n).
- Mỗi bounded context là một service độc lập, có database riêng (PostgreSQL), giao tiếp qua API Gateway (Ocelot) và message bus (RabbitMQ).
- Sử dụng Docker để triển khai và mở rộng, Kubernetes cho production.
- Tích hợp file storage (MinIO) cho import/export dữ liệu.

## Quyết định kỹ thuật chính
- Ưu tiên tích hợp qua API REST, Webhook, hoặc message bus.
- Sử dụng môi trường tách biệt cho phát triển, staging, production.
- Tất cả giao tiếp giữa các service phải qua API Gateway hoặc RabbitMQ (không gọi trực tiếp giữa các service).
- Sử dụng event-driven cho đồng bộ dữ liệu, ưu tiên publish/subscribe, CDC, dual-write pattern với fallback.
- Authentication: OpenID Connect, JWT, OAuth2, RBAC, policy-based authorization.
- Logging tập trung (ELK/EFK), metrics Prometheus, dashboard Grafana, correlation ID.

## Pattern thiết kế
- Modular workflow: mỗi workflow là một module độc lập, có thể mở rộng.
- Sử dụng event-driven cho các tác vụ bất đồng bộ, retry, dead letter queue.
- Mỗi service phải có tài liệu mô tả rõ ràng về API, sự kiện, và database schema.
- Health check endpoint cho từng service, giám sát tự động.
- Backup/restore tự động hóa, kiểm thử định kỳ.
- **Foreign Key Design Pattern: Sử dụng nullable Guid (Guid?) cho tất cả Foreign Keys để tạo mối quan hệ linh hoạt, không quá chặt chẽ giữa các Entity. Điều này cho phép:**
  - **Tạo Entity mà không cần liên kết ngay lập tức với Entity khác**
  - **Xử lý các trường hợp dữ liệu không đầy đủ hoặc import từ nguồn bên ngoài**
  - **Hỗ trợ soft delete và orphaned records management**
  - **Tăng tính linh hoạt trong việc thiết kế API và business logic**

## Quan hệ thành phần
- n8n là trung tâm điều phối workflow.
- Các dịch vụ nghiệp vụ chính:
  - Identity & Access: AuthService, UserService, RoleService
  - **Core Finance: AccountService, TransactionService, StatementService, RecurringTransactionTemplateService, ExpectedTransactionService**
  - Money Management: BudgetService, JarService, SharedExpenseService
  - Planning & Investment: DebtService, GoalService, InvestmentService
  - Reporting & Integration: ReportingService, NotificationService, IntegrationService
  - **ExcelApi: Excel processing services (đã di chuyển vào src/BE/ExcelApi)**
- Mỗi service gắn với database riêng, không chia sẻ schema.
- File storage (MinIO) dùng cho import/export statement.

**Backend Organization:**
- **Tất cả backend services** được tổ chức trong `src/BE/` folder
- **ExcelApi** đã được di chuyển từ `src/ExcelApi/` vào `src/BE/ExcelApi/` để thống nhất cấu trúc
- **PFM.sln** trong `src/BE/` quản lý tất cả backend projects
- **Docker configuration** đã được cập nhật để phù hợp với cấu trúc mới

**Core Finance Services:**
- **RecurringTransactionTemplateService:**
  - Quản lý các mẫu giao dịch định kỳ (RecurringTransactionTemplate).
  - Cung cấp API để tạo/sửa/xóa/liệt kê mẫu giao dịch định kỳ.
  - Sinh ra các giao dịch dự kiến dựa trên mẫu (thông qua background worker hoặc event-driven).
  - Tính toán ngày thực hiện tiếp theo dựa trên frequency và custom interval.
  - Hỗ trợ quản lý trạng thái active/inactive của mẫu.

- **ExpectedTransactionService:**
  - Quản lý các giao dịch dự kiến (ExpectedTransaction) được sinh từ mẫu định kỳ.
  - Cung cấp lifecycle management: Pending → Confirmed/Cancelled/Completed.
  - Hỗ trợ điều chỉnh giao dịch dự kiến (amount, reason) với lưu trữ original amount.
  - Cung cấp dự báo dòng tiền (cash flow forecast) và phân tích theo category.
  - Tương tác với TransactionService khi confirm expected transaction.

## Luồng nghiệp vụ chính
- Import sao kê: User upload file → StatementService lưu file (MinIO) → publish event → StatementProcessor xử lý → TransactionService ghi nhận giao dịch.
- Ghi nhận giao dịch thủ công: User nhập → API Gateway → TransactionService → AccountService cập nhật số dư → publish event cho các service khác (report, budget, ...).
- **Quản lý giao dịch định kỳ:**
  - **User tạo mẫu → RecurringTransactionTemplateService lưu mẫu → background job sinh giao dịch dự kiến → ExpectedTransactionService quản lý lifecycle.**
  - **Background job chạy định kỳ → GenerateExpectedTransactionsForAllActiveTemplatesAsync → sinh giao dịch dự kiến cho tất cả mẫu active.**
  - **User xác nhận giao dịch dự kiến → ExpectedTransactionService.ConfirmTransactionAsync/AdjustTransactionAsync/CancelTransactionAsync → liên kết với Transaction thực tế (Confirm), cập nhật thông tin (Adjust), hoặc đánh dấu đã hủy (Cancel).**
- Quản lý ngân sách, mục tiêu, nợ, đầu tư: các service chuyên biệt, đồng bộ qua event bus.
- Báo cáo, thông báo: ReportingService, NotificationService consume event từ các service khác.

## Best Practices
- Clean Architecture, DDD, TDD, SOLID cho mọi service.
- Viết unit test, integration test đầy đủ, coverage > 80%.
- Chuẩn hóa sử dụng FluentAssertions cho assert kết quả trong unit test, tuân thủ .NET rule.
- **Validation: Sử dụng FluentValidation cho tất cả các request DTO. Các validator được đăng ký tập trung trong lớp `ServiceExtensions` thông qua extension method `AddApplicationValidators`. Validation được thực hiện tự động bởi middleware của ASP.NET Core.**
- **Trong unit test cho tầng service, ưu tiên sử dụng instance AutoMapper thật (được inject, không mock) để kiểm tra logic nghiệp vụ cùng với logic mapping, giả định rằng các AutoMapper profile đã được cấu hình đúng và được test riêng.**
- Triển khai blue-green/canary, feature flag cho release.
- Có tài liệu hướng dẫn backup, restore, disaster recovery.
- Định nghĩa acceptance criteria rõ ràng cho từng chức năng.
- Đảm bảo các yêu cầu phi chức năng: hiệu năng, mở rộng, bảo mật, usability, reliability, compliance.
- **Quy ước tổ chức file Unit Test: Sử dụng partial class với tên `ServiceNameTests` trải rộng trên nhiều file. Các file con, đặt tên theo định dạng `ServiceNameTests.MethodName.cs`, sẽ được nhóm vào một thư mục con cùng tên với lớp test (`ServiceNameTests`) bên trong thư mục test chính (`CoreFinance.Application.Tests`). Các hàm helper dùng chung cho test sẽ được đặt trong file `TestHelpers.cs` trong thư mục `Helpers`.**

## Lựa chọn giải pháp
- Import statement: ưu tiên manual upload (CSV/Excel), lên kế hoạch tích hợp API ngân hàng hoặc aggregator trong tương lai.
- Frontend: (Cần xác định, ví dụ React, Angular, Vue, Blazor).
- **Sinh giao dịch định kỳ: sử dụng background worker định kỳ (nightly job) với Quartz.NET hoặc Hangfire, kết hợp với event-driven khi tạo mẫu mới để có kết quả ngay lập tức.**
- **Domain models: RecurringTransactionTemplate và ExpectedTransaction được thiết kế với đầy đủ navigation properties để hỗ trợ Entity Framework relationships.**

## Lộ trình & Ưu tiên
- Triển khai theo phase: core service trước (Identity, Core Finance, Reporting), sau đó Money Management, Planning & Investment, cuối cùng là advanced features.
- **Core Finance đã hoàn thành: Account, Transaction, RecurringTransactionTemplate, ExpectedTransaction services.**
- **Tiếp theo: triển khai API Controllers, background job service, tích hợp với NotificationService và ReportingService.**
- Mỗi phase có checklist hoàn thành, review kiến trúc và bảo mật.

## Luồng triển khai quan trọng
- Tự động hóa backup/restore.
- CI/CD cho workflow và cấu hình.
- Health check, logging, monitoring, alerting tự động.
- **Background job scheduling cho việc sinh giao dịch dự kiến từ các mẫu định kỳ.**
- **Event publishing khi có thay đổi trong expected transactions để đồng bộ với reporting và notification services.** 