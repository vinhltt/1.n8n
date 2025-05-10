#!/bin/bash
# deploy-n8n.staging.sh - Script triển khai n8n cho môi trường staging

# Hiển thị banner
echo "=============================================="
echo "      TRIỂN KHAI N8N SERVER - STAGING        "
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

# 1. Kiểm tra file .env
log "🔍 Kiểm tra file .env..."
if [ ! -f ".env" ]; then
  handle_error "Không tìm thấy file .env"
fi

# 2. Chuẩn bị môi trường
log "🧹 Đang chuẩn bị môi trường..."

# Tạo thư mục n8n_data_staging trước nếu chưa tồn tại
if [ ! -d "./n8n_data_staging" ]; then
  mkdir -p ./n8n_data_staging
  log "✅ Đã tạo thư mục n8n_data_staging"
fi

# Sao lưu file config hiện tại nếu có
if [ -f "./n8n_data_staging/config" ]; then
  log "📦 Sao lưu file config hiện tại..."
  cp "./n8n_data_staging/config" "./n8n_data_staging/config.bak"
  log "✅ Đã sao lưu file config"
fi

# Kiểm tra file config hiện tại xem có chứa chú thích không mong muốn không
CONFIG_ISSUE=false
if [ -f "./n8n_data_staging/config" ]; then
  if grep -q "# <<<=== THAY BẰNG KHÓA MẠNH CỦA BẠN" "./n8n_data_staging/config"; then
    log "⚠️ Phát hiện chú thích không mong muốn trong file config hiện tại"
    CONFIG_ISSUE=true
  fi
fi

# CHỈ tạo file config mới nếu không tồn tại hoặc có vấn đề
if [ ! -f "./n8n_data_staging/config" ] || [ "$CONFIG_ISSUE" = true ]; then
  if [ -f ".env" ]; then
    CLEAN_KEY=$(grep "N8N_ENCRYPTION_KEY" .env | cut -d '=' -f2 | xargs)
    if [ ! -z "$CLEAN_KEY" ]; then
      log "📝 Tạo file config với encryptionKey từ .env..."
      echo "{\"encryptionKey\": \"$CLEAN_KEY\"}" > ./n8n_data_staging/config
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

# 3. Build Excel API image
log "🔨 Đang build Excel API image..."
cd src/ExcelApi
docker build -t pfm-excel-api:latest . || handle_error "Không thể build Excel API image"
cd ../..
log "✅ Đã build Excel API image thành công"

# 4. Pull PostgreSQL image
log "📥 Đang pull PostgreSQL image..."
docker pull postgres:15 || handle_error "Không thể pull PostgreSQL image"
log "✅ Đã pull PostgreSQL image thành công"

# 5. Dừng container hiện tại (nếu có)
log "⏹️ Dừng containers hiện tại..."
docker-compose -f docker-compose.yml -f docker-compose.staging.yml down || true
log "✅ Đã dừng các containers hiện tại"

# 6. GIỮ NGUYÊN VOLUMES - Không xóa volumes nữa
log "✅ Giữ nguyên dữ liệu n8n hiện có"

# 7. Khởi động container
log "🚀 Đang khởi động containers..."
docker-compose -f docker-compose.yml -f docker-compose.staging.yml up -d || handle_error "Không thể khởi động containers"
log "✅ Đã khởi động containers thành công"

# 8. Đợi n8n khởi động
log "⏳ Đang đợi n8n khởi động..."
sleep 15
log "✅ Đã đợi đủ thời gian cho n8n khởi động"

# 9. Kiểm tra trạng thái container
log "🔍 Kiểm tra trạng thái container n8n..."
if ! docker ps | grep -q "n8n_main_staging"; then
  log "⚠️ Container n8n_main_staging không đang chạy, kiểm tra logs..."
  docker logs n8n_main_staging --tail 50
  handle_error "Container n8n_main_staging không khởi động được"
fi
log "✅ Container n8n_main_staging đang chạy"

# 10. Kiểm tra file config
log "🔍 Kiểm tra file config của n8n..."
CONFIG_CONTENT=$(docker exec n8n_main_staging cat /home/node/.n8n/config 2>/dev/null)
if [ $? -ne 0 ]; then
  log "⚠️ Không thể đọc file config, có thể file chưa được tạo"
else
  echo "$CONFIG_CONTENT"
  log "✅ Đã đọc file config thành công"
fi

# 11. Kiểm tra thư viện msoffcrypto-tool
log "🔍 Kiểm tra cài đặt thư viện msoffcrypto-tool..."
if docker exec n8n_main_staging bash -c "pip3 list | grep -q msoffcrypto-tool"; then
  MSOFFCRYPTO_VERSION=$(docker exec n8n_main_staging bash -c "pip3 list | grep msoffcrypto-tool" | awk '{print $2}')
  log "✅ Thư viện msoffcrypto-tool đã được cài đặt (phiên bản $MSOFFCRYPTO_VERSION)"
else
  log "⚠️ Thư viện msoffcrypto-tool chưa được cài đặt đúng cách"
fi

# 12. Kiểm tra trạng thái các container
log "🔍 Kiểm tra trạng thái các container..."
docker-compose -f docker-compose.yml -f docker-compose.staging.yml ps

# 13. Hiển thị URL truy cập n8n
log "🌐 n8n đã được triển khai thành công và có thể truy cập tại:"
echo "   http://localhost:5679"
if [ ! -z "$WEBHOOK_URL" ]; then
  echo "   hoặc: $WEBHOOK_URL"
fi

log "✅ TRIỂN KHAI N8N STAGING HOÀN TẤT THÀNH CÔNG"
exit 0 