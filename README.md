# n8n Main/Worker Architecture với Redis

Hệ thống n8n với kiến trúc main/worker sử dụng Redis làm message queue để xử lý workflows với sequential processing.

## 🚀 Services

- **n8n-main**: Main service xử lý UI và API
- **n8n-worker**: Worker service xử lý workflows (sequential processing)  
- **Redis**: Message queue cho main/worker communication
- **PostgreSQL**: Database cho n8n
- **Excel API**: API xử lý Excel files

## 📦 Docker Commands

### Khởi động tất cả services:

```bash
docker-compose up -d
```

### Dừng và xóa containers:

```bash
docker-compose down
```

### Cập nhật images:

```bash
docker-compose pull
```

## 📊 Monitoring

```bash
# Xem trạng thái tất cả services
docker-compose ps

# Xem logs real-time
docker-compose logs -f

# Xem logs của service cụ thể  
docker-compose logs -f n8n-main
docker-compose logs -f n8n-worker
docker-compose logs -f redis
```

## 🔧 Troubleshooting

### n8n Worker không hoạt động:
- Kiểm tra Redis connection
- Kiểm tra worker concurrency settings
- Xem logs: `docker-compose logs n8n-worker`

### n8n Webhook không nhận được dữ liệu:
- Kiểm tra workflow có được kích hoạt không
- Kiểm tra URL webhook có đúng không
- Kiểm tra network connectivity giữa services

--- 
