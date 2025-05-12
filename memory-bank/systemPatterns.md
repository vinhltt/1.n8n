# systemPatterns.md

## Kiến trúc hệ thống
- Microservices kết hợp với workflow engine (n8n).
- Sử dụng Docker để triển khai và mở rộng.

## Quyết định kỹ thuật chính
- Ưu tiên tích hợp qua API REST, Webhook.
- Sử dụng môi trường tách biệt cho phát triển, staging, production.

## Pattern thiết kế
- Modular workflow: mỗi workflow là một module độc lập.
- Sử dụng event-driven cho các tác vụ bất đồng bộ.

## Quan hệ thành phần
- n8n là trung tâm điều phối.
- Các dịch vụ bên ngoài (API, DB, file, v.v.) kết nối qua node.

## Luồng triển khai quan trọng
- Tự động hóa backup/restore.
- CI/CD cho workflow và cấu hình. 