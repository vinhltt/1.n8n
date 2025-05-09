#!/bin/bash
# deploy-n8n.prod.sh - Script triển khai n8n cho môi trường production

# Hiển thị banner
echo "=============================================="
echo "     TRIỂN KHAI N8N SERVER - PRODUCTION      "
echo "=============================================="

# Ghi log với timestamp
log() {
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
}

# Xử lý lỗi
handle_error() {
  log "❌ Lỗi: $1"
  exit 1
}

# 1. Chuẩn bị môi trường
log "🧹 Đang chuẩn bị môi trường..."

# Tạo thư mục n8n_data trước nếu chưa tồn tại
if [ ! -d "./n8n_data" ]; then
  mkdir -p ./n8n_data
  log "✅ Đã tạo thư mục n8n_data"
fi

# Sao lưu file config hiện tại nếu có
if [ -f "./n8n_data/config" ]; then
  log "📦 Sao lưu file config hiện tại..."
  cp "./n8n_data/config" "./n8n_data/config.bak"
  log "✅ Đã sao lưu file config"
fi

# Kiểm tra file config hiện tại xem có chứa chú thích không mong muốn không
CONFIG_ISSUE=false
if [ -f "./n8n_data/config" ]; then
  if grep -q "# <<<=== THAY BẰNG KHÓA MẠNH CỦA BẠN" "./n8n_data/config"; then
    log "⚠️ Phát hiện chú thích không mong muốn trong file config hiện tại"
    CONFIG_ISSUE=true
  fi
fi

# CHỈ tạo file config mới nếu không tồn tại hoặc có vấn đề
if [ ! -f "./n8n_data/config" ] || [ "$CONFIG_ISSUE" = true ]; then
  if [ -f ".env" ]; then
    CLEAN_KEY=$(grep "N8N_ENCRYPTION_KEY" .env | cut -d '=' -f2 | xargs)
    if [ ! -z "$CLEAN_KEY" ]; then
      log "📝 Tạo file config với encryptionKey từ .env..."
      echo "{\"encryptionKey\": \"$CLEAN_KEY\"}" > ./n8n_data/config
      log "✅ Đã tạo file config mới"
    else
      log "⚠️ Không tìm thấy N8N_ENCRYPTION_KEY trong file .env"
    fi
  else
    log "⚠️ Không tìm thấy file .env"
  fi
else
  log "✅ File config hiện tại không có vấn đề, giữ nguyên"
fi

# 2. Pull images mới nhất cho các dịch vụ khác
log "🔄 Đang pull images mới nhất..."
docker-compose -f docker-compose.yml -f docker-compose.prod.yml pull || handle_error "Không thể pull images"
log "✅ Đã pull images thành công"

# 3. Build images for all services defined in docker-compose.yml
log "🔨 Đang build images cho tất cả các dịch vụ..."
docker-compose -f docker-compose.yml -f docker-compose.prod.yml build || handle_error "Không thể build images"
log "✅ Đã build images thành công"

# 4. Dừng container hiện tại (nếu có)
log "⏹️ Dừng containers hiện tại..."
docker-compose -f docker-compose.yml -f docker-compose.prod.yml down || true
log "✅ Đã dừng các containers hiện tại"

# 5. GIỮ NGUYÊN VOLUMES - Không xóa volumes nữa
log "✅ Giữ nguyên dữ liệu n8n hiện có"

# 6. Khởi động container
log "🚀 Đang khởi động containers..."
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d || handle_error "Không thể khởi động containers"
log "✅ Đã khởi động containers thành công"

# 7. Đợi n8n khởi động
log "⏳ Đang đợi n8n khởi động..."
sleep 15
log "✅ Đã đợi đủ thời gian cho n8n khởi động"

# 8. Kiểm tra trạng thái container
log "🔍 Kiểm tra trạng thái container n8n..."
if ! docker ps | grep -q "n8n_main"; then
  log "⚠️ Container n8n_main không đang chạy, kiểm tra logs..."
  docker logs n8n_main --tail 50
  handle_error "Container n8n_main không khởi động được"
fi
log "✅ Container n8n_main đang chạy"

# 9. Kiểm tra file config
log "🔍 Kiểm tra file config của n8n..."
CONFIG_CONTENT=$(docker exec n8n_main cat /home/node/.n8n/config 2>/dev/null)
if [ $? -ne 0 ]; then
  log "⚠️ Không thể đọc file config, có thể file chưa được tạo"
else
  echo "$CONFIG_CONTENT"
  log "✅ Đã đọc file config thành công"
fi

# 10. Kiểm tra thư viện msoffcrypto-tool
log "🔍 Kiểm tra cài đặt thư viện msoffcrypto-tool..."
if docker exec n8n_main bash -c "pip3 list | grep -q msoffcrypto-tool"; then
  MSOFFCRYPTO_VERSION=$(docker exec n8n_main bash -c "pip3 list | grep msoffcrypto-tool" | awk '{print $2}')
  log "✅ Thư viện msoffcrypto-tool đã được cài đặt (phiên bản $MSOFFCRYPTO_VERSION)"
else
  log "⚠️ Thư viện msoffcrypto-tool chưa được cài đặt đúng cách"
fi

# 11. Kiểm tra trạng thái các container
log "🔍 Kiểm tra trạng thái các container..."
docker-compose -f docker-compose.yml -f docker-compose.prod.yml ps

# 12. Hiển thị URL truy cập n8n
log "🌐 n8n đã được triển khai thành công và có thể truy cập tại:"
echo "   http://localhost:5678"
if [ ! -z "$WEBHOOK_URL" ]; then
  echo "   hoặc: $WEBHOOK_URL"
fi

log "✅ TRIỂN KHAI N8N PRODUCTION HOÀN TẤT THÀNH CÔNG"
exit 0 