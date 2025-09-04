# 🚀 GitHub Actions Setup cho n8n Main/Worker

## 📋 Tổng quan

File này hướng dẫn cấu hình GitHub Actions để tự động deploy n8n main/worker architecture lên TrueNAS qua Cloudflared.

## 🔐 GitHub Secrets (Repository Settings > Secrets and variables > Actions)

### Secrets cần thiết:

```env
# TrueNAS SSH Connection
TRUENAS_SSH_PRIVATE_KEY=<private_key_content>
TRUENAS_SSH_HOSTNAME_THROUGH_CLOUDFLARED=<your_cloudflared_hostname>
TRUENAS_USER=<truenas_username>

# Database
POSTGRES_USER=n8n
POSTGRES_PASSWORD=<strong_password>
POSTGRES_DB=n8n_database

# n8n Configuration
N8N_BASIC_AUTH_USER=admin
N8N_BASIC_AUTH_PASSWORD=<strong_password>
N8N_ENCRYPTION_KEY=<generate_with_openssl_rand_hex_32>

# Redis Configuration
REDIS_PASSWORD=<strong_redis_password>
REDIS_DB=0

# n8n Main/Worker Configuration
N8N_BASIC_AUTH_ACTIVE=true
N8N_CONCURRENCY=1

# Excel API Configuration  
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# Optional System Settings
GENERIC_TIMEZONE=Asia/Ho_Chi_Minh
TZ=Asia/Ho_Chi_Minh
N8N_DEFAULT_BINARY_DATA_MODE=filesystem
EXECUTIONS_DATA_PRUNE=true
EXECUTIONS_DATA_MAX_AGE=720
EXECUTIONS_DATA_PRUNE_MAX_COUNT=50000
N8N_BACKUP_DIR_HOST=<backup_directory_path>

# Notification Webhook (optional)
NOTIFICATION_WEBHOOK_URL=<webhook_url_for_notifications>
```

## 🔧 GitHub Variables (Repository Settings > Secrets and variables > Actions)

### Variables cần thiết:

```env
# Project
COMPOSE_PROJECT_NAME=n8n

# Network  
IP_PREFIX=172.20.0

# Ports
POSTGRES_EXTERNAL_PORT=5432
N8N_EXTERNAL_PORT=5678
EXCEL_API_HTTP_PORT=8080
EXCEL_API_HTTPS_PORT=8443
REDIS_EXTERNAL_PORT=6379

# URLs & Paths
WEBHOOK_URL=https://your-domain.com
DEPLOY_PATH_ON_TRUENAS=/path/to/deploy

# n8n Configuration
N8N_RETENTION=7

# Discord (optional)
DISCORD_N8N_WEBHOOK_URL=http://n8n:5678/webhook/discord-event
```

## 📦 Redis Configuration

### Redis Password Generation

```bash
# Generate strong Redis password
openssl rand -hex 32
```

Thêm password này vào GitHub Secrets với tên `REDIS_PASSWORD`.

## 🔄 Workflow Triggers

Workflow sẽ tự động chạy khi:
- Push code lên branch `master`, `develop`, hoặc `staging`
- Trigger thủ công từ GitHub Actions tab

## 📊 Quá trình Deploy

1. **Checkout code** từ repository
2. **Setup Cloudflared** và SSH config
3. **Sync files** lên TrueNAS
4. **Tạo .env file** với tất cả biến môi trường
5. **Prepare directories** cho n8n main/worker setup
6. **Docker operations**: pull, build, down, up
7. **Verify n8n services** deployment
8. **Send notifications** qua webhook

## 🧪 Test Deployment

### Kiểm tra sau khi deploy:

```bash
# SSH vào TrueNAS
ssh your-truenas-user@your-truenas-ip

# Kiểm tra containers
cd /path/to/deploy/deploy_branch_name
docker compose ps

# Xem logs n8n main
docker compose logs -f n8n-main

# Xem logs n8n worker
docker compose logs -f n8n-worker

# Xem logs redis
docker compose logs -f redis
```

### Kiểm tra n8n:

1. ✅ n8n main service online
2. ✅ n8n worker connected to Redis
3. ✅ Test workflow execution trên worker

## 🔍 Troubleshooting

### n8n Worker không hoạt động:

```bash
# Kiểm tra logs
docker compose logs n8n-worker

# Kiểm tra Redis connection
docker compose logs redis
```

### Common Issues:

1. **Redis connection failed**: Kiểm tra REDIS_PASSWORD trong GitHub Secrets
2. **Worker không execute**: Kiểm tra concurrency settings
3. **Webhook không hoạt động**: Kiểm tra n8n workflow đã kích hoạt
4. **Network issues**: Kiểm tra IP_PREFIX và network config

## 📝 Environment Files

### Cấu trúc file .env được tạo tự động:

```env
# Project name
COMPOSE_PROJECT_NAME=n8n

# PostgreSQL
POSTGRES_USER=n8n
POSTGRES_PASSWORD=***
POSTGRES_DB=n8n_database
POSTGRES_EXTERNAL_PORT=5432

# n8n
N8N_BASIC_AUTH_USER=admin
N8N_BASIC_AUTH_PASSWORD=***
N8N_ENCRYPTION_KEY=***
DB_POSTGRESDB_PASSWORD=***
WEBHOOK_URL=https://your-domain.com
N8N_EXTERNAL_PORT=5678

# Excel API
EXCEL_API_HTTP_PORT=8080
EXCEL_API_HTTPS_PORT=8443

# Redis Configuration
REDIS_PASSWORD=***
REDIS_DB=0
REDIS_EXTERNAL_PORT=6379

# n8n Main/Worker Configuration  
N8N_BASIC_AUTH_ACTIVE=true
N8N_CONCURRENCY=1

# Excel API Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# Timezone & other settings
GENERIC_TIMEZONE=Asia/Ho_Chi_Minh
TZ=Asia/Ho_Chi_Minh
N8N_DEFAULT_BINARY_DATA_MODE=filesystem
EXECUTIONS_DATA_PRUNE=true
EXECUTIONS_DATA_MAX_AGE=720
EXECUTIONS_DATA_PRUNE_MAX_COUNT=50000

# Backup settings
N8N_BACKUP_DIR_HOST=/path/to/backup
IP_PREFIX=172.20.0
TRUENAS_DEPLOY_DIR=/path/to/deploy
```

## 🎯 Next Steps

1. ✅ Cấu hình tất cả GitHub Secrets và Variables
2. ✅ Generate Redis password và lưu vào GitHub Secrets
3. ✅ Setup notification webhook cho thông báo (optional)
4. ✅ Tạo n8n workflow cho testing worker functionality
5. ✅ Push code để trigger deployment
6. ✅ Monitor logs và test sequential processing

---

💡 **Tip**: Monitor worker logs để đảm bảo "Concurrency: 1" được hiển thị và sequential processing hoạt động đúng. 