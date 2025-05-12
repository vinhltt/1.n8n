# techContext.md

## Công nghệ sử dụng
- n8n (workflow automation)
- Docker, docker-compose
- Node.js, TypeScript (nếu mở rộng custom node)
- PostgreSQL (mặc định cho n8n)

## Thiết lập phát triển
- Sử dụng file .env cho cấu hình môi trường
- Phân chia môi trường: dev, staging, production
- Hỗ trợ backup/restore qua script

## Ràng buộc kỹ thuật
- Phải chạy ổn định trên Linux và Windows
- Hạn chế tối đa downtime khi triển khai

## Dependency
- n8n >= 1.0
- Docker >= 20.x
- PostgreSQL >= 13

## Pattern sử dụng công cụ
- Ưu tiên dùng docker-compose cho local dev/test
- Script hóa các thao tác lặp lại (backup, restore, deploy) 