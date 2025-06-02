# Discord Bot cho n8n Triggers

Discord bot này được thiết kế để lắng nghe các sự kiện từ Discord server và gửi webhook đến n8n để trigger các workflows tự động.

## 🚀 Tính năng

- ✅ Lắng nghe tin nhắn mới (`message_create`)
- ✅ Lắng nghe thành viên mới tham gia (`member_join`)
- ✅ Lắng nghe thành viên rời khỏi server (`member_leave`)
- ✅ Lắng nghe reactions (`reaction_add`)
- ✅ Gửi webhook đến n8n với dữ liệu chi tiết
- ✅ Logging với nhiều level (error, warn, info, debug)
- ✅ Cấu hình linh hoạt qua biến môi trường
- ✅ Hỗ trợ Docker deployment

## 📋 Yêu cầu

- Node.js 16+ hoặc Docker
- Discord Bot Token
- n8n instance với webhook endpoint

## 🔧 Cài đặt

### Bước 1: Tạo Discord Bot

1. Truy cập [Discord Developer Portal](https://discord.com/developers/applications)
2. Tạo **New Application** và đặt tên (ví dụ: `n8n Trigger Bot`)
3. Vào tab **Bot** → **Add Bot**
4. Bật **Message Content Intent** trong **Privileged Gateway Intents**
5. Copy **Token** để sử dụng

### Bước 2: Mời Bot vào Server

1. Vào tab **OAuth2** → **URL Generator**
2. Chọn scope: `bot`
3. Chọn permissions:
   - Read Messages
   - Send Messages
   - View Channels
   - Read Message History
4. Copy URL và mời bot vào server Discord của bạn

### Bước 3: Cấu hình n8n Webhook

1. Trong n8n, tạo workflow mới
2. Thêm node **Webhook**:
   - Method: `POST`
   - Path: `discord-event` (hoặc tùy chỉnh)
3. Kích hoạt workflow
4. Copy URL webhook (dạng: `https://your-n8n-domain/webhook/discord-event`)

### Bước 4: Cấu hình Bot

1. Copy file `env.example` thành `.env`:
   ```bash
   cp env.example .env
   ```

2. Chỉnh sửa file `.env`:
   ```env
   DISCORD_TOKEN=your_discord_bot_token_here
   N8N_WEBHOOK_URL=https://your-n8n-domain/webhook/discord-event
   ENABLE_MESSAGE_LOGGING=true
   ENABLE_MEMBER_JOIN_LOGGING=true
   LOG_LEVEL=info
   ```

## 🏃‍♂️ Chạy Bot

### Chạy với Node.js

```bash
# Cài đặt dependencies
npm install

# Chạy bot
npm start

# Hoặc chạy trong development mode
npm run dev
```

### Chạy với Docker

```bash
# Build image
docker build -t discord-n8n-bot .

# Chạy container
docker run -d \
  --name discord-bot \
  --env-file .env \
  --restart unless-stopped \
  discord-n8n-bot
```

### Tích hợp với Docker Compose hiện tại

Thêm service sau vào file `docker-compose.yml`:

```yaml
discord-bot:
  build:
    context: ./discord-bot
    dockerfile: Dockerfile
  restart: always
  environment:
    - DISCORD_TOKEN=${DISCORD_TOKEN}
    - N8N_WEBHOOK_URL=http://n8n:5678/webhook/discord-event
    - ENABLE_MESSAGE_LOGGING=${DISCORD_ENABLE_MESSAGE_LOGGING:-true}
    - ENABLE_MEMBER_JOIN_LOGGING=${DISCORD_ENABLE_MEMBER_JOIN_LOGGING:-true}
    - LOG_LEVEL=${DISCORD_LOG_LEVEL:-info}
    - GUILD_ID=${DISCORD_GUILD_ID:-}
  depends_on:
    - n8n
  networks:
    - custom_network
```

Và thêm các biến môi trường vào file `.env`:

```env
# Discord Bot Configuration
DISCORD_TOKEN=your_discord_bot_token_here
DISCORD_ENABLE_MESSAGE_LOGGING=true
DISCORD_ENABLE_MEMBER_JOIN_LOGGING=true
DISCORD_LOG_LEVEL=info
DISCORD_GUILD_ID=your_discord_server_id_optional
```

## 📊 Dữ liệu Webhook

Bot sẽ gửi các loại dữ liệu sau đến n8n:

### Message Create Event
```json
{
  "type": "message_create",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "message": {
    "id": "message_id",
    "content": "Hello world!",
    "author": {
      "id": "user_id",
      "username": "username",
      "displayName": "Display Name",
      "bot": false
    },
    "channel": {
      "id": "channel_id",
      "name": "general",
      "type": 0
    },
    "guild": {
      "id": "guild_id",
      "name": "Server Name"
    },
    "attachments": [],
    "mentions": {
      "users": [],
      "roles": [],
      "everyone": false
    }
  }
}
```

### Member Join Event
```json
{
  "type": "member_join",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "member": {
    "id": "user_id",
    "username": "new_user",
    "displayName": "New User",
    "bot": false,
    "avatar": "https://cdn.discordapp.com/avatars/...",
    "joinedAt": "2024-01-01T12:00:00.000Z",
    "accountCreatedAt": "2023-01-01T12:00:00.000Z"
  },
  "guild": {
    "id": "guild_id",
    "name": "Server Name",
    "memberCount": 150
  }
}
```

## 🔧 Cấu hình

| Biến môi trường | Mô tả | Mặc định |
|---|---|---|
| `DISCORD_TOKEN` | Token của Discord bot | **Bắt buộc** |
| `N8N_WEBHOOK_URL` | URL webhook của n8n | **Bắt buộc** |
| `BOT_PREFIX` | Prefix cho bot commands | `!` |
| `ENABLE_MESSAGE_LOGGING` | Bật/tắt logging tin nhắn | `true` |
| `ENABLE_MEMBER_JOIN_LOGGING` | Bật/tắt logging member join | `true` |
| `GUILD_ID` | ID server Discord cụ thể | `null` (tất cả servers) |
| `LOG_LEVEL` | Level logging | `info` |

## 🧪 Test

1. Gửi tin nhắn trong Discord server
2. Kiểm tra logs của bot
3. Kiểm tra n8n workflow có được trigger không
4. Thêm/xóa thành viên để test member events

## 🔧 Troubleshooting

### Lỗi đăng nhập Discord Bot

#### 1. "TokenInvalid" hoặc "Không thể đăng nhập Discord bot"

**Nguyên nhân:**
- Discord Token không hợp lệ hoặc đã hết hạn
- Token không được cấu hình đúng trong biến môi trường

**Cách khắc phục:**
1. Kiểm tra Discord Token tại [Discord Developer Portal](https://discord.com/developers/applications)
2. Đảm bảo bot đã được tạo và token được copy chính xác
3. Kiểm tra biến môi trường `DISCORD_TOKEN` trong file `.env` hoặc GitHub Secrets

```bash
# Kiểm tra token trong container
docker compose exec discord-bot printenv DISCORD_TOKEN
```

#### 2. Bot restart liên tục

**Nguyên nhân:**
- Token không hợp lệ khiến bot exit và Docker restart
- Lỗi kết nối mạng

**Cách khắc phục:**
1. Kiểm tra logs: `docker compose logs discord-bot`
2. Kiểm tra restart policy trong `docker-compose.yml` (đã được set thành `on-failure:3`)
3. Xác minh token Discord hợp lệ

#### 3. Bot không gửi webhook đến n8n

**Nguyên nhân:**
- `N8N_WEBHOOK_URL` không được cấu hình
- n8n service chưa sẵn sàng
- Network connectivity issues

**Cách khắc phục:**
1. Kiểm tra biến `N8N_WEBHOOK_URL` trong `.env`
2. Đảm bảo n8n đang chạy: `docker compose ps n8n`
3. Test webhook endpoint từ container:

```bash
docker compose exec discord-bot curl -X POST http://n8n:5678/webhook/discord-event \
  -H "Content-Type: application/json" \
  -d '{"test": "message"}'
```

### Debug Commands

```bash
# Xem logs realtime
docker compose logs -f discord-bot

# Kiểm tra biến môi trường
docker compose exec discord-bot printenv | grep DISCORD

# Restart chỉ Discord bot
docker compose restart discord-bot

# Kiểm tra network connectivity
docker compose exec discord-bot ping n8n
```

### GitHub Actions Deployment Issues

#### 1. Secrets không được cấu hình

Đảm bảo các secrets sau được cấu hình tại `https://github.com/YOUR_REPO/settings/secrets/actions`:

- `DISCORD_TOKEN`: Token của Discord Bot
- `POSTGRES_PASSWORD`: Mật khẩu PostgreSQL  
- `N8N_ENCRYPTION_KEY`: Khóa mã hóa n8n

#### 2. Deployment thành công nhưng bot không hoạt động

1. Kiểm tra logs trong GitHub Actions
2. SSH vào server và kiểm tra:

```bash
cd /path/to/deploy/directory
docker compose logs discord-bot
docker compose ps
```

## 📝 Logs

Bot sử dụng structured logging với các level:
- `error`: Lỗi nghiêm trọng
- `warn`: Cảnh báo
- `info`: Thông tin chung
- `debug`: Chi tiết debug

Để xem logs chi tiết, set `LOG_LEVEL=debug`.

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch
3. Commit changes
4. Push to branch
5. Tạo Pull Request

## 📄 License

MIT License - xem file LICENSE để biết thêm chi tiết. 