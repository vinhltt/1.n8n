# techContext.md

## Công nghệ sử dụng
- Backend: .NET Core (C#), ASP.NET Core
- API Gateway: Ocelot
- Message Bus: RabbitMQ
- Database: PostgreSQL (mỗi service một DB riêng)
- Workflow engine: n8n
- File Storage: MinIO
- Containerization: Docker, docker-compose cho dev, Kubernetes cho production
- Logging/Monitoring: ELK/EFK, Prometheus, Grafana
- Authentication: OpenIddict, JWT, OAuth2
- (Frontend): (Cần xác định - ví dụ: React, Angular, Vue, Blazor)
- FluentAssertions cho assert kết quả trong unit test
- **Quartz.NET hoặc Hangfire cho xử lý recurring jobs và lịch trình giao dịch định kỳ**
- **Scheduling library: NCrontab, Cronos hoặc Quartz.NET cho việc xử lý các quy tắc lặp lại phức tạp trong RecurringTransactionService**

## Thiết lập phát triển
- Sử dụng file .env cho cấu hình môi trường
- Phân chia môi trường: dev, staging, production
- Hỗ trợ backup/restore qua script, kiểm thử định kỳ
- CI/CD tự động build, test, deploy, rollback
- Hạ tầng mô tả bằng code (IaC), ưu tiên cloud-native, auto-scaling

## Ràng buộc kỹ thuật
- Phải chạy ổn định trên Linux và Windows
- Hạn chế tối đa downtime khi triển khai
- Đảm bảo hiệu năng: API chính <2s với 1000 user đồng thời, import 1000 dòng <30s
- Đảm bảo bảo mật: tuân thủ OWASP Top 10, mã hóa dữ liệu nhạy cảm, RBAC, policy-based authorization
- Đảm bảo maintainability: Clean Architecture, SOLID, test coverage >80%
- Đảm bảo usability: UI trực quan, thao tác chính không quá 3 bước
- Đảm bảo reliability: backup/restore định kỳ, event-driven sync, retry mechanism
- Đảm bảo compliance: (Cần xác định quy định pháp lý nếu có)

## Dependency
- n8n >= 1.0
- Docker >= 20.x
- PostgreSQL >= 13
- RabbitMQ >= 3.x
- Ocelot >= 17.x
- OpenIddict >= 4.x
- **Quartz.NET hoặc Hangfire >= 2.x (cho RecurringTransactionService)**
- (Frontend framework: TBD)

## Pattern sử dụng công cụ
- Ưu tiên dùng docker-compose cho local dev/test
- Script hóa các thao tác lặp lại (backup, restore, deploy)
- Tích hợp health check, logging, monitoring, alerting tự động
- **Sử dụng NCrontab hoặc Cronos để đọc/viết chuỗi cron expression cho việc xác định tần suất lặp của giao dịch định kỳ** 