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

## Quan hệ thành phần
- n8n là trung tâm điều phối workflow.
- Các dịch vụ nghiệp vụ chính:
  - Identity & Access: AuthService, UserService, RoleService
  - Core Finance: AccountService, TransactionService, StatementService
  - Money Management: BudgetService, JarService, SharedExpenseService
  - Planning & Investment: DebtService, GoalService, InvestmentService, RecurringTransactionService
  - Reporting & Integration: ReportingService, NotificationService, IntegrationService
- Mỗi service gắn với database riêng, không chia sẻ schema.
- File storage (MinIO) dùng cho import/export statement.

RecurringTransactionService:
- Quản lý các mẫu giao dịch định kỳ (RecurringTransactionTemplate) và giao dịch dự kiến (ExpectedTransaction).
- Cung cấp API để tạo/sửa/xóa/liệt kê mẫu giao dịch định kỳ.
- Sinh ra các giao dịch dự kiến dựa trên mẫu (thông qua background worker hoặc event-driven).
- Publish events như ExpectedTransactionCreated, DueDateApproaching cho các service khác.
- Tương tác với ReportingService để cung cấp dữ liệu cho báo cáo kế hoạch tiền mặt.
- Tương tác với NotificationService để gửi thông báo về giao dịch định kỳ sắp đến hạn.

## Luồng nghiệp vụ chính
- Import sao kê: User upload file → StatementService lưu file (MinIO) → publish event → StatementProcessor xử lý → TransactionService ghi nhận giao dịch.
- Ghi nhận giao dịch thủ công: User nhập → API Gateway → TransactionService → AccountService cập nhật số dư → publish event cho các service khác (report, budget, ...).
- Quản lý ngân sách, mục tiêu, nợ, đầu tư: các service chuyên biệt, đồng bộ qua event bus.
- Báo cáo, thông báo: ReportingService, NotificationService consume event từ các service khác.
- Quản lý giao dịch định kỳ: User tạo mẫu → RecurringTransactionService lưu mẫu → sinh giao dịch dự kiến → publish event → ReportingService tạo báo cáo kế hoạch tiền mặt, NotificationService gửi thông báo.

## Best Practices
- Clean Architecture, DDD, TDD, SOLID cho mọi service.
- Viết unit test, integration test đầy đủ, coverage > 80%.
- Chuẩn hóa sử dụng FluentAssertions cho assert kết quả trong unit test, tuân thủ .NET rule.
- **Trong unit test cho tầng service, ưu tiên sử dụng instance AutoMapper thật (được inject, không mock) để kiểm tra logic nghiệp vụ cùng với logic mapping, giả định rằng các AutoMapper profile đã được cấu hình đúng và được test riêng.**
- Triển khai blue-green/canary, feature flag cho release.
- Có tài liệu hướng dẫn backup, restore, disaster recovery.
- Định nghĩa acceptance criteria rõ ràng cho từng chức năng.
- Đảm bảo các yêu cầu phi chức năng: hiệu năng, mở rộng, bảo mật, usability, reliability, compliance.
- Quy ước tổ chức file Unit Test: Sử dụng **partial class** với tên `ServiceNameTests` trải rộng trên nhiều file. Các file con, đặt tên theo định dạng `ServiceNameTests.MethodName.cs`, sẽ được nhóm vào một thư mục con cùng tên với lớp test (`ServiceNameTests`) bên trong thư mục test chính (`CoreFinance.Application.Tests`). Các hàm helper dùng chung cho test sẽ được đặt trong file `TestHelpers.cs` trong thư mục `Helpers`.

## Lựa chọn giải pháp
- Import statement: ưu tiên manual upload (CSV/Excel), lên kế hoạch tích hợp API ngân hàng hoặc aggregator trong tương lai.
- Frontend: (Cần xác định, ví dụ React, Angular, Vue, Blazor).
- Sinh giao dịch định kỳ: ưu tiên background worker định kỳ (nightly job) kết hợp với event-driven khi tạo mẫu mới để có kết quả ngay lập tức.

## Lộ trình & Ưu tiên
- Triển khai theo phase: core service trước (Identity, Core Finance, Reporting), sau đó Money Management, Planning & Investment, cuối cùng là advanced features.
- Mỗi phase có checklist hoàn thành, review kiến trúc và bảo mật.

## Luồng triển khai quan trọng
- Tự động hóa backup/restore.
- CI/CD cho workflow và cấu hình.
- Health check, logging, monitoring, alerting tự động. 