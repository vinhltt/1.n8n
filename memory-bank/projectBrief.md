# Project Brief: n8n Automation Platform với Backup System

## Mục tiêu chính
Dự án triển khai hệ thống n8n automation platform với Discord bot integration và hệ thống backup tự động.

## Kiến trúc hệ thống
- **n8n**: Workflow automation platform
- **PostgreSQL**: Database lưu trữ dữ liệu n8n
- **Discord Bot**: Tích hợp với n8n qua webhook
- **Excel API**: Service xử lý Excel files
- **TrueNAS Server**: Môi trường production qua Cloudflared tunnel

## Deployment Strategy
- **GitHub Actions**: CI/CD pipeline tự động
- **Docker Compose**: Container orchestration
- **Cloudflared**: Secure tunnel kết nối đến TrueNAS
- **Multi-environment**: master, develop, staging branches

## Backup System
- **Daily backup**: Chạy hàng ngày lúc 7:15 sáng (UTC+7)
- **PostgreSQL dump**: Backup database với compression
- **Volume backup**: Archive Docker volume data
- **Retention**: Giữ backup 7 ngày
- **SSH tunnel**: Backup thực thi trên TrueNAS qua Cloudflared

## Vấn đề hiện tại
User báo cáo lỗi trong quá trình backup tự động qua GitHub Actions workflow.