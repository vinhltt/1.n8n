# n8n với Discord Bot Integration

Hệ thống n8n tích hợp Discord bot để tự động trigger workflows từ các sự kiện Discord.

## 🚀 Services

- **n8n**: Workflow automation platform
- **PostgreSQL**: Database cho n8n
- **Excel API**: API xử lý Excel files
- **Discord Bot**: Bot lắng nghe sự kiện Discord và trigger n8n workflows

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

### Xem logs của Discord bot:

```bash
docker-compose logs -f discord-bot
```

## 🤖 Discord Bot Setup

Xem hướng dẫn chi tiết trong file: **[DISCORD-BOT-SETUP.md](./DISCORD-BOT-SETUP.md)**

### Setup nhanh:

1. **Tạo Discord bot** tại [Discord Developer Portal](https://discord.com/developers/applications)
2. **Cấu hình .env** với Discord token
3. **Tạo webhook trong n8n** với path `discord-event`
4. **Chạy hệ thống**: `docker-compose up -d`

### Cấu hình .env:

```env
# Discord Bot
DISCORD_TOKEN=your_discord_bot_token
DISCORD_N8N_WEBHOOK_URL=http://n8n:5678/webhook/discord-event
DISCORD_ENABLE_MESSAGE_LOGGING=true
DISCORD_ENABLE_MEMBER_JOIN_LOGGING=true
```

## 🔗 Tích hợp

Discord bot sẽ gửi các sự kiện sau đến n8n:

- ✅ **Message Create**: Khi có tin nhắn mới
- ✅ **Member Join**: Khi có thành viên mới tham gia
- ✅ **Member Leave**: Khi thành viên rời khỏi server  
- ✅ **Reaction Add**: Khi có reaction mới

## 📊 Monitoring

```bash
# Xem trạng thái tất cả services
docker-compose ps

# Xem logs real-time
docker-compose logs -f

# Xem logs của service cụ thể
docker-compose logs -f discord-bot
docker-compose logs -f n8n
```

## 🔧 Troubleshooting

### Discord Bot không hoạt động:
- Kiểm tra `DISCORD_TOKEN` trong file `.env`
- Kiểm tra bot có được mời vào Discord server không
- Xem logs: `docker-compose logs discord-bot`

### n8n Webhook không nhận được dữ liệu:
- Kiểm tra workflow có được kích hoạt không
- Kiểm tra URL webhook có đúng không
- Kiểm tra network connectivity giữa services

---

💡 **Tip**: Sử dụng `DISCORD_LOG_LEVEL=debug` để xem chi tiết hoạt động của bot trong quá trình setup. 
