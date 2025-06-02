# 🚀 GitHub Actions Setup cho n8n + Discord Bot

## 📋 Tổng quan

File này hướng dẫn cấu hình GitHub Actions để tự động deploy n8n và Discord bot lên TrueNAS qua Cloudflared.

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

# Discord Bot (MỚI)
DISCORD_TOKEN=<your_discord_bot_token>

# Optional
GENERIC_TIMEZONE=Asia/Ho_Chi_Minh
TZ=Asia/Ho_Chi_Minh
N8N_DEFAULT_BINARY_DATA_MODE=filesystem
EXECUTIONS_DATA_PRUNE=true
EXECUTIONS_DATA_MAX_AGE=720
EXECUTIONS_DATA_PRUNE_MAX_COUNT=50000
N8N_BACKUP_DIR_HOST=<backup_directory_path>

# Discord Webhook cho thông báo deploy
DISCORD_WEBHOOK_URL=<discord_webhook_url_for_notifications>
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

# URLs
WEBHOOK_URL=https://your-domain.com
DEPLOY_PATH_ON_TRUENAS=/path/to/deploy

# Discord Bot Configuration (MỚI)
DISCORD_N8N_WEBHOOK_URL=http://n8n:5678/webhook/discord-event
DISCORD_BOT_PREFIX=!
DISCORD_ENABLE_MESSAGE_LOGGING=true
DISCORD_ENABLE_MEMBER_JOIN_LOGGING=true
DISCORD_LOG_LEVEL=info
DISCORD_GUILD_ID=<optional_discord_server_id>
```

## 🤖 Discord Bot Setup

### 1. Tạo Discord Bot Token

1. Truy cập [Discord Developer Portal](https://discord.com/developers/applications)
2. Tạo **New Application** → đặt tên `n8n Trigger Bot`
3. Vào tab **Bot** → **Add Bot**
4. **QUAN TRỌNG**: Bật **Message Content Intent**
5. Copy **Token** → thêm vào GitHub Secrets với tên `DISCORD_TOKEN`

### 2. Cấu hình Discord Webhook cho thông báo

1. Trong Discord server, vào Settings → Integrations → Webhooks
2. Tạo webhook mới cho channel thông báo
3. Copy webhook URL → thêm vào GitHub Secrets với tên `DISCORD_WEBHOOK_URL`

## 🔄 Workflow Triggers

Workflow sẽ tự động chạy khi:
- Push code lên branch `master`, `develop`, hoặc `staging`
- Trigger thủ công từ GitHub Actions tab

## 📊 Quá trình Deploy

1. **Checkout code** từ repository
2. **Setup Cloudflared** và SSH config
3. **Sync files** lên TrueNAS
4. **Tạo .env file** với tất cả biến môi trường
5. **Prepare directories** cho n8n và Discord bot
6. **Docker operations**: pull, build, down, up
7. **Verify Discord Bot** deployment
8. **Send notifications** qua Discord webhook

## 🧪 Test Deployment

### Kiểm tra sau khi deploy:

```bash
# SSH vào TrueNAS
ssh your-truenas-user@your-truenas-ip

# Kiểm tra containers
cd /path/to/deploy/deploy_branch_name
docker compose ps

# Xem logs Discord bot
docker compose logs -f discord-bot

# Xem logs n8n
docker compose logs -f n8n
```

### Kiểm tra Discord Bot:

1. ✅ Bot online trong Discord server
2. ✅ Gửi tin nhắn test → kiểm tra n8n webhook
3. ✅ Thêm member mới → kiểm tra member join event

## 🔍 Troubleshooting

### Discord Bot không hoạt động:

```bash
# Kiểm tra logs
docker compose logs discord-bot

# Kiểm tra biến môi trường
docker compose exec discord-bot env | grep DISCORD
```

### Common Issues:

1. **DISCORD_TOKEN invalid**: Kiểm tra token trong GitHub Secrets
2. **Bot không online**: Kiểm tra Message Content Intent
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

# Discord Bot Configuration
DISCORD_TOKEN=***
DISCORD_N8N_WEBHOOK_URL=http://n8n:5678/webhook/discord-event
DISCORD_BOT_PREFIX=!
DISCORD_ENABLE_MESSAGE_LOGGING=true
DISCORD_ENABLE_MEMBER_JOIN_LOGGING=true
DISCORD_LOG_LEVEL=info
DISCORD_GUILD_ID=

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
2. ✅ Tạo Discord bot và lấy token
3. ✅ Setup Discord webhook cho thông báo
4. ✅ Tạo n8n workflow với webhook endpoint
5. ✅ Push code để trigger deployment
6. ✅ Monitor logs và test functionality

---

💡 **Tip**: Bắt đầu với `DISCORD_LOG_LEVEL=debug` để xem chi tiết, sau đó chuyển về `info` khi ổn định. 