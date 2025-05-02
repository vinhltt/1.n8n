#!/bin/bash
# clean-n8n-docker.sh - Script xóa toàn bộ dữ liệu n8n trong Docker

echo "🧹 Bắt đầu xóa toàn bộ dữ liệu n8n trong Docker..."

# Dừng tất cả các container đang chạy
echo "⏹️ Dừng tất cả các container..."
docker-compose down
echo "✅ Đã dừng tất cả các container"

# Xóa thư mục n8n_data nếu tồn tại trong thư mục hiện tại
echo "🗑️ Xóa thư mục n8n_data..."
rm -rf ./n8n_data
echo "✅ Đã xóa thư mục n8n_data"

# Xóa file .env nếu tồn tại
echo "🗑️ Xóa file .env..."
rm -f .env
echo "✅ Đã xóa file .env"

# Xóa volumes Docker
echo "🗑️ Xóa volumes Docker..."
docker volume rm n8n_data postgresdb_data || true
echo "✅ Đã xóa volumes Docker"

# Xóa images Docker (tùy chọn)
echo "🗑️ Xóa images Docker liên quan đến n8n..."
docker rmi docker.n8n.io/n8nio/n8n postgres:15 || true
echo "✅ Đã xóa images Docker"

# Xóa networks
echo "🗑️ Xóa networks Docker..."
docker network rm n8n-network || true
echo "✅ Đã xóa networks Docker"

# Tạo lại thư mục n8n_data và ssh
echo "📁 Tạo lại thư mục n8n_data và ssh..."
mkdir -p ./n8n_data
mkdir -p ./ssh
echo "✅ Đã tạo lại các thư mục cần thiết"

echo "🔄 Kích hoạt GitHub Action để triển khai n8n..."
echo "1. Truy cập GitHub repository của bạn"
echo "2. Chọn tab 'Actions'"
echo "3. Chọn workflow 'Auto Deploy n8n'"
echo "4. Nhấn nút 'Run workflow'"

echo "✨ Hoàn tất! Docker đã được làm sạch, sẵn sàng cho việc triển khai lại n8n."