# N8N Main/Worker Architecture with Redis

Cấu hình này triển khai N8N theo kiến trúc Main/Worker với Redis làm message queue, cho phép scale horizontal và xử lý workflow hiệu quả hơn.

## 🏗️ Kiến trúc

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   N8N Main      │    │   N8N Worker    │    │   N8N Worker    │
│  (Web UI + API) │    │   (Execution)   │    │   (Execution)   │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         └───────────────────────┼───────────────────────┘
                                 │
                    ┌─────────────────┐
                    │      Redis      │
                    │  (Message Queue)│
                    │                 │
                    └─────────────────┘
                                 │
                    ┌─────────────────┐
                    │   PostgreSQL    │
                    │   (Database)    │
                    │                 │
                    └─────────────────┘
```

## 🚀 Cách sử dụng

### 1. Cấu hình Environment Variables

Sao chép file `.env.example` thành `.env` và cập nhật các giá trị:

```bash
cp .env.example .env
```

**Cấu hình quan trọng:**
- `REDIS_PASSWORD`: Mật khẩu Redis (bắt buộc)
- `N8N_ENCRYPTION_KEY`: Key mã hóa N8N (32 ký tự)
- `POSTGRES_PASSWORD`: Mật khẩu PostgreSQL
- `IP_PREFIX`: Dải IP cho containers (ví dụ: 192.168.100)

### 2. Khởi chạy Services

```bash
# Khởi chạy tất cả services
sudo docker-compose up -d

# Kiểm tra trạng thái
sudo docker-compose ps
```

### 3. Scale Workers

Sử dụng script scale để điều chỉnh số lượng workers:

```bash
# Scale to 3 workers
./scale-n8n-workers.sh scale 3

# Kiểm tra trạng thái workers
./scale-n8n-workers.sh status

# Stop all workers
./scale-n8n-workers.sh stop-all
```

### 4. Monitoring

```bash
# Health check cơ bản
./monitor-n8n-health.sh

# Chi tiết với resource usage
./monitor-n8n-health.sh detailed

# Monitoring liên tục
./monitor-n8n-health.sh watch
```

## 📦 Services và Ports

| Service | Port | IP Address | Mô tả |
|---------|------|------------|-------|
| N8N Main | 5678 | ${IP_PREFIX}.4 | Web UI + API |
| PostgreSQL | 5432 | ${IP_PREFIX}.3 | Database |
| Redis | 6379 | ${IP_PREFIX}.6 | Message Queue |
| N8N Workers | - | ${IP_PREFIX}.7+ | Workflow Execution |

## 🔧 Backup & Restore

### Backup

```bash
# Backup với script mới (hỗ trợ Redis)
./backup-n8n-main-worker.sh
```

### Restore

```bash
# Restore từ backup directory
./restore-n8n-main-worker.sh ./backups/20250829_143022
```

## ⚙️ Cấu hình nâng cao

### Redis Configuration

Các biến môi trường Redis quan trọng:
- `REDIS_PASSWORD`: Mật khẩu xác thực
- `REDIS_DB`: Database number (mặc định: 0)
- `REDIS_EXTERNAL_PORT`: Port external (mặc định: 6379)

### N8N Queue Configuration

N8N sử dụng Redis để quản lý queue:
- `EXECUTIONS_MODE=queue`: Bật chế độ queue
- `QUEUE_BULL_REDIS_HOST=redis`: Redis host
- `N8N_WORKERS_COUNT=0`: Main instance không chạy workers

### Worker Scaling

Workers có thể scale từ 0-10 instances:
```bash
# Scale to specific number
./scale-n8n-workers.sh scale 5

# Restart all workers
./scale-n8n-workers.sh restart
```

## 🔍 Troubleshooting

### 1. Services không start

```bash
# Check logs
sudo docker-compose logs [service-name]

# Check health
./monitor-n8n-health.sh detailed
```

### 2. Redis connection issues

```bash
# Test Redis connection
sudo docker exec $(sudo docker-compose ps -q redis) redis-cli -a YOUR_PASSWORD ping
```

### 3. Workers không nhận jobs

```bash
# Check worker logs
sudo docker-compose logs n8n-worker

# Check Redis queue
sudo docker exec $(sudo docker-compose ps -q redis) redis-cli -a YOUR_PASSWORD monitor
```

### 4. Performance optimization

- **CPU**: Scale workers theo số CPU cores
- **Memory**: Monitor usage với `./monitor-n8n-health.sh detailed`
- **Network**: Ensure Redis và PostgreSQL có bandwidth đủ

## 📊 Monitoring Commands

```bash
# Quick health check
./monitor-n8n-health.sh

# Resource usage
./monitor-n8n-health.sh detailed

# Real-time monitoring
./monitor-n8n-health.sh watch

# Worker scaling status
./scale-n8n-workers.sh status
```

## 🔐 Security Notes

1. **Redis Password**: Đặt mật khẩu mạnh cho Redis
2. **Network Isolation**: Chỉ expose ports cần thiết
3. **Database Security**: Secure PostgreSQL credentials
4. **File Permissions**: Đảm bảo scripts có quyền thực thi phù hợp

## 🆕 Migration từ Single Instance

Để migrate từ cấu hình single instance sang main/worker:

1. **Backup data hiện tại**:
   ```bash
   ./backup-n8n.sh  # Script cũ
   ```

2. **Update docker-compose.yml**: Đã được cập nhật với Redis và workers

3. **Update environment variables**: Thêm Redis config vào `.env`

4. **Deploy mới**:
   ```bash
   sudo docker-compose down
   sudo docker-compose up -d
   ```

5. **Restore data**:
   ```bash
   ./restore-n8n-main-worker.sh ./backups/BACKUP_DIRECTORY
   ```

## 🚧 Best Practices

1. **Worker Scaling**: Start với 1-2 workers, scale theo nhu cầu
2. **Resource Monitoring**: Sử dụng monitoring script thường xuyên
3. **Backup Strategy**: Chạy backup daily với script mới
4. **Redis Memory**: Monitor Redis memory usage
5. **PostgreSQL Tuning**: Optimize cho concurrent connections từ workers

## 📝 Files được cập nhật

- `docker-compose.yml`: Thêm Redis, Main/Worker architecture
- `.env.example`: Template với Redis config
- `backup-n8n-main-worker.sh`: Backup script mới
- `restore-n8n-main-worker.sh`: Restore script mới
- `scale-n8n-workers.sh`: Worker scaling script
- `monitor-n8n-health.sh`: Health monitoring script
