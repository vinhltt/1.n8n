# Active Context - Vấn đề Docker Permissions trong Backup Flow

## 🚨 Vấn đề hiện tại: GitHub Actions Backup Failure

### Mô tả lỗi
- GitHub Actions workflow `n8n-backup.yml` đang bị lỗi permission denied
- Log error: `permission denied while trying to connect to the Docker daemon socket at unix:///var/run/docker.sock`
- Backup script `backup-n8n.sh` không thể thực thi Docker commands trên TrueNAS server

### Nguyên nhân root cause
1. User trên TrueNAS server không có quyền truy cập Docker daemon socket
2. Script backup cần chạy các commands: `docker compose ps`, `docker exec`, `docker run`, `docker inspect`
3. User chưa được thêm vào Docker group hoặc thiếu sudo permissions

## 🛠️ Giải pháp đã áp dụng

### 1. Sửa backup script với sudo
- Đã update `backup-n8n.sh` để thêm `sudo` prefix cho tất cả Docker commands
- Các commands được sửa:
  - `docker-compose ps` → `sudo docker-compose ps`
  - `docker compose ps` → `sudo docker compose ps`
  - `docker ps` → `sudo docker ps`
  - `docker inspect` → `sudo docker inspect`
  - `docker exec` → `sudo docker exec`
  - `docker run` → `sudo docker run`

### 2. Giải pháp thay thế (khuyến nghị dài hạn)
Thêm user vào Docker group trên TrueNAS server:
```bash
sudo usermod -aG docker $USER
newgrp docker
```

## 📋 Next Steps
1. Test backup script với sudo permissions
2. Xem xét setup passwordless sudo cho Docker commands
3. Monitor GitHub Actions workflow để ensure stability
4. Document troubleshooting steps for future issues

## 🔄 Workflow Context
- GitHub Actions chạy daily backup lúc 7:15 sáng (UTC+7)
- Backup script connect qua SSH tunnel (Cloudflared)
- TrueNAS server host các Docker containers: n8n, PostgreSQL, Discord bot
- Backup bao gồm: database dump + Docker volume archive

## ⚠️ Risk & Considerations
- Sudo permissions có thể tạo security risk
- Cần ensure user có proper sudo configuration
- Monitor disk space cho backup retention policy 