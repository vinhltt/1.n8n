#!/bin/bash
# deep-clean-n8n.sh - Xóa triệt để n8n khỏi Docker và filesystem

echo "===== BẮT ĐẦU XÓA TRIỆT ĐỂ N8N ====="

# 1. Dừng và xóa tất cả container
echo "🛑 Dừng và xóa tất cả container..."
docker-compose down || true
echo "✅ Đã dừng các container"

# 2. Xóa dữ liệu n8n_data trên filesystem
echo "🗑️ Xóa thư mục n8n_data trên filesystem..."
rm -rf ./n8n_data
echo "✅ Đã xóa thư mục n8n_data"

# 3. Xóa file .env
echo "🗑️ Xóa file .env..."
rm -f .env
rm -f .env.* # Xóa tất cả các file .env.* khác
echo "✅ Đã xóa file .env"

# 4. Xóa volumes Docker
echo "🗑️ Xóa volumes Docker..."
docker volume rm n8n_data postgresdb_data || true
echo "✅ Đã xóa volumes Docker"

# 5. Xóa tất cả các container có tên chứa n8n
echo "🗑️ Xóa tất cả container liên quan đến n8n..."
docker ps -a | grep 'n8n\|postgres' | awk '{print $1}' | xargs -r docker rm -f || true
echo "✅ Đã xóa các container liên quan"

# 6. Xóa và tải lại images để đảm bảo sạch hoàn toàn
echo "🗑️ Xóa images Docker..."
docker rmi docker.n8n.io/n8nio/n8n postgres:15 || true
echo "✅ Đã xóa images"

# 7. Xóa network liên quan đến n8n
echo "🗑️ Xóa networks Docker..."
docker network rm n8n-network || true
echo "✅ Đã xóa networks"

# 8. Tìm và xóa tất cả các file config và .env ẩn trong thư mục làm việc
echo "🔍 Tìm và xóa các file config và .env ẩn..."
find . -name "config" -o -name ".env*" -type f | grep -v "node_modules\|.git" | xargs -r rm -fv
echo "✅ Đã xóa các file ẩn"

# 9. Kiểm tra thư mục home của runner cho các config ẩn
echo "🔍 Kiểm tra thư mục home của runner..."
rm -f $HOME/.env $HOME/.n8n/config 2>/dev/null || true
echo "✅ Đã kiểm tra thư mục home"

# 10. Tạo lại thư mục n8n_data trống
echo "📁 Tạo thư mục n8n_data mới..."
mkdir -p ./n8n_data
mkdir -p ./ssh
echo "✅ Đã tạo thư mục mới"

# 11. Xóa các thư mục cache và temp của Docker
echo "🧹 Xóa cache Docker..."
docker system prune -f || true
echo "✅ Đã xóa cache Docker"

# 12. Tìm kiếm các vị trí khác có thể lưu trữ cấu hình cũ
echo "🔍 Tìm kiếm các vị trí lưu trữ cấu hình khác..."
find $HOME/actions-runner -name "config" -o -name ".env*" 2>/dev/null | grep -v "node_modules\|.git" | xargs -r rm -fv || true
echo "✅ Đã kiểm tra các vị trí lưu trữ khác"

echo "===== XÓA HOÀN TẤT ====="
echo "🔄 Bây giờ bạn có thể kích hoạt GitHub Action để triển khai lại n8n với cấu hình mới"