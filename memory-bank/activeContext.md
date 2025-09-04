# Active Context - N8N Main/Worker Architecture Migration

## � Vấn đề hiện tại: N8N Main/Worker với Redis Setup

### Mô tả cập nhật
- Đã migrate N8N từ single instance sang kiến trúc Main/Worker với Redis
- Cấu hình mới hỗ trợ horizontal scaling và xử lý workflow hiệu quả hơn
- Redis được sử dụng làm message queue giữa Main instance và Workers

### Architecture mới
1. **N8N Main instance** - xử lý Web UI, API, và scheduling
2. **N8N Worker instance(s)** - xử lý execution workflows
3. **Redis** - message queue và cache
4. **PostgreSQL** - database (không thay đổi)

## 🛠️ Giải pháp đã áp dụng

### 1. Cập nhật docker-compose.yml
- Thêm Redis service với static IP ${IP_PREFIX}.6
- Tách N8N thành 2 services: n8n-main và n8n-worker
- N8N Main: ${IP_PREFIX}.4 (Web UI + scheduling)
- N8N Worker: ${IP_PREFIX}.7 (workflow execution)
- Cấu hình Redis queue cho cả main và worker instances

### 2. Environment Variables mới
- `REDIS_PASSWORD`: Mật khẩu Redis bảo mật
- `REDIS_DB`: Redis database number
- `EXECUTIONS_MODE=queue`: Bật chế độ queue
- `N8N_WORKERS_COUNT=0`: Main instance không chạy workers
- `QUEUE_BULL_REDIS_*`: Cấu hình Redis connection

### 3. Scripts mới được tạo
- `backup-n8n-main-worker.sh`: Backup bao gồm Redis data
- `restore-n8n-main-worker.sh`: Restore cho architecture mới
- `scale-n8n-workers.sh`: Scale workers dynamically (0-10 instances)
- `monitor-n8n-health.sh`: Health monitoring cho tất cả services
- `.env.example`: Template với Redis configuration

## 📋 Next Steps
1. Test cấu hình mới với migration từ single instance
2. Validate backup/restore process với Redis data
3. Performance testing với multiple workers
4. Update GitHub Actions workflow cho backup script mới
5. Monitor resource usage và optimize worker scaling

## 🔄 Migration Workflow
1. **Backup current data**: Sử dụng script backup cũ
2. **Update .env**: Thêm Redis config từ .env.example
3. **Deploy new architecture**: docker-compose up -d
4. **Restore data**: Sử dụng restore script mới
5. **Scale workers**: Sử dụng scale script theo nhu cầu

## ⚠️ Risk & Considerations
- **Data Migration**: Cần test kỹ backup/restore process
- **Resource Usage**: Workers tiêu tốn thêm memory và CPU
- **Redis Security**: Đảm bảo Redis password được set mạnh
- **Network Latency**: Monitor latency giữa services trong queue
- **Scaling Strategy**: Start với 1-2 workers, scale theo monitoring

## 🔧 Operational Commands
```bash
# Health check
./monitor-n8n-health.sh

# Scale workers
./scale-n8n-workers.sh scale 3

# Backup (mới)
./backup-n8n-main-worker.sh

# Restore (mới)
./restore-n8n-main-worker.sh ./backups/BACKUP_DIR
``` 