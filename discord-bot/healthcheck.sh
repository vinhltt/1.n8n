#!/bin/sh

# Discord Bot Health Check Script
# Kiểm tra bot có đang chạy và connected không

# Kiểm tra process node
if ! pgrep -f "node.*index.js" > /dev/null; then
    echo "❌ Node process not running"
    exit 1
fi

# Kiểm tra log file có lỗi gần đây không (nếu có)
if [ -f "/tmp/bot.log" ]; then
    if tail -n 10 /tmp/bot.log | grep -q "ERROR.*không thể đăng nhập"; then
        echo "❌ Bot login failed"
        exit 1
    fi
fi

# Kiểm tra Discord API từ container
if command -v curl > /dev/null; then
    if [ ! -z "$DISCORD_TOKEN" ]; then
        HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" \
            -H "Authorization: Bot $DISCORD_TOKEN" \
            "https://discord.com/api/v10/users/@me")
        
        if [ "$HTTP_CODE" != "200" ]; then
            echo "❌ $HTTP_CODE Discord API not reachable or token invalid"
            exit 1
        fi
    fi
fi

echo "✅ Bot is healthy"
exit 0
