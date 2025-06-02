#!/bin/bash

# Discord Bot Startup Script
# Author: VinhLTT
# Description: Script để khởi động Discord bot cho n8n triggers

set -e

echo "🤖 Discord Bot cho n8n Triggers"
echo "================================"

# Kiểm tra file .env
if [ ! -f ".env" ]; then
    echo "❌ File .env không tồn tại!"
    echo "📝 Vui lòng copy từ env.example và cấu hình:"
    echo "   cp env.example .env"
    echo "   nano .env"
    exit 1
fi

# Load biến môi trường
source .env

# Kiểm tra DISCORD_TOKEN
if [ -z "$DISCORD_TOKEN" ]; then
    echo "❌ DISCORD_TOKEN chưa được cấu hình trong file .env"
    echo "📝 Vui lòng thêm Discord bot token vào file .env"
    exit 1
fi

# Kiểm tra N8N_WEBHOOK_URL
if [ -z "$N8N_WEBHOOK_URL" ]; then
    echo "⚠️  N8N_WEBHOOK_URL chưa được cấu hình"
    echo "📝 Bot sẽ chạy nhưng không gửi webhook đến n8n"
fi

# Kiểm tra Node.js
if ! command -v node &> /dev/null; then
    echo "❌ Node.js chưa được cài đặt"
    echo "📦 Vui lòng cài đặt Node.js 16+ từ: https://nodejs.org/"
    exit 1
fi

# Kiểm tra npm
if ! command -v npm &> /dev/null; then
    echo "❌ npm chưa được cài đặt"
    exit 1
fi

# Cài đặt dependencies nếu chưa có
if [ ! -d "node_modules" ]; then
    echo "📦 Đang cài đặt dependencies..."
    npm install
fi

echo "✅ Tất cả kiểm tra đã hoàn thành"
echo "🚀 Đang khởi động Discord bot..."
echo ""

# Khởi động bot
npm start 