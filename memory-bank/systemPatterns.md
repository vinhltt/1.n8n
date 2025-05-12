# systemPatterns.md

## Kiến trúc hệ thống
- Microservices kết hợp với workflow engine (n8n).
- Mỗi bounded context là một service độc lập, có database riêng, giao tiếp qua API Gateway và message bus (RabbitMQ).
- Sử dụng Docker để triển khai và mở rộng.

## Quyết định kỹ thuật chính
- Ưu tiên tích hợp qua API REST, Webhook, hoặc message bus.
- Sử dụng môi trường tách biệt cho phát triển, staging, production.
- Tất cả giao tiếp giữa các service phải qua API Gateway hoặc RabbitMQ (không gọi trực tiếp giữa các service).
- Sử dụng event-driven cho đồng bộ dữ liệu, ưu tiên publish/subscribe, CDC, dual-write pattern với fallback.

## Pattern thiết kế
- Modular workflow: mỗi workflow là một module độc lập.
- Sử dụng event-driven cho các tác vụ bất đồng bộ.
- Mỗi service phải có tài liệu mô tả rõ ràng về API, sự kiện, và database schema.

## Quan hệ thành phần
- n8n là trung tâm điều phối.
- Các dịch vụ bên ngoài (API, DB, file, v.v.) kết nối qua node.

## Bảo mật
- Áp dụng OpenID Connect, JWT, OAuth2 cho xác thực.
- Phân quyền dựa trên RBAC, claims, policy.
- Không lưu thông tin nhạy cảm dưới dạng plain text.
- Kiểm tra bảo mật định kỳ, quét dependency, kiểm thử xâm nhập.

## Logging, Monitoring & Healthcheck
- Tất cả service phải có health endpoint, log tập trung, gắn correlation ID.
- Thu thập metrics Prometheus, dashboard Grafana cho từng service.
- Log phải có cấu trúc, dễ truy vết, không log thông tin nhạy cảm.

## DevOps & Triển khai
- Mỗi service phải có Dockerfile, hỗ trợ chạy độc lập.
- CI/CD tự động build, test, deploy, rollback.
- Hạ tầng mô tả bằng code (IaC), ưu tiên cloud-native, auto-scaling.
- Backup và disaster recovery phải được kiểm thử định kỳ.

## Best Practices
- Áp dụng Clean Architecture, DDD, TDD, SOLID cho mọi service.
- Viết unit test, integration test đầy đủ.
- Triển khai blue-green/canary, feature flag cho release.
- Có tài liệu hướng dẫn backup, restore, disaster recovery.

## Lộ trình & Ưu tiên
- Triển khai theo phase, ưu tiên core service trước, advanced feature sau.
- Mỗi phase phải có checklist hoàn thành, review kiến trúc và bảo mật.

## Luồng triển khai quan trọng
- Tự động hóa backup/restore.
- CI/CD cho workflow và cấu hình. 