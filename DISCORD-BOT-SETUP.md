# 🤖 Hướng dẫn Setup Discord Bot cho n8n

## 📋 Tổng quan

Discord bot này sẽ lắng nghe các sự kiện từ Discord server và gửi webhook đến n8n để trigger workflows tự động.

## 🚀 Setup nhanh

### 1. Tạo Discord Bot

1. Truy cập [Discord Developer Portal](https://discord.com/developers/applications)
2. Tạo **New Application** → đặt tên `n8n Trigger Bot`
3. Vào tab **Bot** → **Add Bot**
4. **QUAN TRỌNG**: Bật **Message Content Intent** trong **Privileged Gateway Intents**
5. Copy **Token** để sử dụng

### 2. Mời Bot vào Server

1. Vào tab **OAuth2** → **URL Generator**
2. Chọn scope: `bot`
3. Chọn permissions:
   - ✅ Read Messages
   - ✅ Send Messages  
   - ✅ View Channels
   - ✅ Read Message History
4. Copy URL và mời bot vào Discord server

### 3. Cấu hình n8n Webhook

1. Trong n8n, tạo workflow mới
2. Thêm node **Webhook**:
   - Method: `POST`
   - Path: `discord-event`
3. **Kích hoạt workflow** (quan trọng!)
4. URL webhook sẽ có dạng: `https://your-domain/webhook/discord-event`

### 4. Cấu hình Bot

Thêm các biến sau vào file `.env` của bạn:

```env
# Discord Bot Configuration
DISCORD_TOKEN=your_discord_bot_token_here
DISCORD_N8N_WEBHOOK_URL=http://n8n:5678/webhook/discord-event
DISCORD_ENABLE_MESSAGE_LOGGING=true
DISCORD_ENABLE_MEMBER_JOIN_LOGGING=true
DISCORD_LOG_LEVEL=info

# Optional: Giới hạn bot chỉ hoạt động trong server cụ thể
# DISCORD_GUILD_ID=your_discord_server_id
```

### 5. Chạy Bot

#### Với Docker Compose (Khuyến nghị)

```bash
# Build và chạy tất cả services bao gồm Discord bot
docker-compose up -d

# Xem logs của Discord bot
docker-compose logs -f discord-bot
```

#### Chạy riêng Discord bot

```bash
cd discord-bot

# Cài đặt dependencies
npm install

# Chạy bot
npm start

# Hoặc sử dụng script tự động
./start.sh
```

## 📊 Dữ liệu nhận được trong n8n

Bot sẽ gửi các loại event sau:

### Message Event
```json
{
  "type": "message_create",
  "message": {
    "content": "Hello world!",
    "author": {
      "username": "user123",
      "id": "123456789"
    },
    "channel": {
      "name": "general"
    }
  }
}
```

### Member Join Event
```json
{
  "type": "member_join",
  "member": {
    "username": "new_user",
    "id": "987654321"
  },
  "guild": {
    "name": "My Server",
    "memberCount": 150
  }
}
```

## 🧪 Test

1. ✅ Gửi tin nhắn trong Discord → kiểm tra n8n workflow có trigger không
2. ✅ Thêm người mới vào server → kiểm tra member join event
3. ✅ Xem logs: `docker-compose logs discord-bot`

## 🔧 Troubleshooting

### Bot không online
- ❌ Kiểm tra `DISCORD_TOKEN` có đúng không
- ❌ Bot có được mời vào server với đúng permissions không

### Không nhận được webhook
- ❌ n8n workflow có được **kích hoạt** không
- ❌ URL webhook có đúng không
- ❌ Kiểm tra network connectivity

### Logs để debug
```bash
# Xem logs chi tiết
docker-compose logs -f discord-bot

# Hoặc set LOG_LEVEL=debug trong .env
```

## 📁 Cấu trúc Files

```
discord-bot/
├── index.js          # Main bot code
├── package.json      # Dependencies
├── Dockerfile        # Docker configuration
├── env.example       # Environment template
├── start.sh          # Quick start script
└── README.md         # Detailed documentation
```

## 🎯 Ví dụ Workflow n8n

Sau khi nhận webhook từ Discord, bạn có thể:

1. **Gửi email thông báo** khi có tin nhắn mới
2. **Tạo task trong Notion** khi có member mới
3. **Gửi tin nhắn tự động** phản hồi
4. **Lưu vào database** để phân tích
5. **Trigger các automation khác**

---

💡 **Tip**: Bắt đầu với `DISCORD_LOG_LEVEL=debug` để xem chi tiết hoạt động của bot, sau đó chuyển về `info` khi đã ổn định.