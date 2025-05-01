#!/bin/bash
# deploy-n8n.sh - Script triển khai n8n với khả năng xử lý lỗi nâng cao

# Hiển thị banner
echo "=============================================="
echo "           TRIỂN KHAI N8N SERVER             "
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

# Tạo thư mục n8n_data trước để sở hữu file config
if [ ! -d "./n8n_data" ]; then
  mkdir -p ./n8n_data
  log "✅ Đã tạo thư mục n8n_data"
fi

# Tạo file config trước khi khởi động container, với giá trị từ .env
if [ -f ".env" ]; then
  CLEAN_KEY=$(grep "N8N_ENCRYPTION_KEY" .env | cut -d '=' -f2 | xargs)
  if [ ! -z "$CLEAN_KEY" ]; then
    log "📝 Tạo file config với encryptionKey từ .env..."
    echo "{\"encryptionKey\": \"$CLEAN_KEY\"}" > ./n8n_data/config
    log "✅ Đã tạo file config"
  else
    log "⚠️ Không tìm thấy N8N_ENCRYPTION_KEY trong file .env"
  fi
else
  log "⚠️ Không tìm thấy file .env"
fi

# 2. Pull images mới nhất
log "🔄 Đang pull images mới nhất..."
docker-compose pull || handle_error "Không thể pull images"
log "✅ Đã pull images thành công"

# 3. Dừng container hiện tại (nếu có)
log "⏹️ Dừng containers hiện tại..."
docker-compose down || true
log "✅ Đã dừng các containers hiện tại"

# 4. Kiểm tra và xóa volumes n8n_data nếu cần
log "🔍 Kiểm tra volumes Docker..."
if docker volume ls | grep -q "n8n_data"; then
  log "🗑️ Xóa volume n8n_data cũ..."
  docker volume rm n8n_data || true
fi

# 5. Khởi động container
log "🚀 Đang khởi động containers..."
docker-compose up -d || handle_error "Không thể khởi động containers"
log "✅ Đã khởi động containers thành công"

# 6. Đợi n8n khởi động
log "⏳ Đang đợi n8n khởi động..."
sleep 15
log "✅ Đã đợi đủ thời gian cho n8n khởi động"

# 7. Kiểm tra trạng thái container
log "🔍 Kiểm tra trạng thái container n8n..."
if ! docker ps | grep -q "n8n_main"; then
  log "⚠️ Container n8n_main không đang chạy, kiểm tra logs..."
  docker logs n8n_main --tail 50
  handle_error "Container n8n_main không khởi động được"
fi
log "✅ Container n8n_main đang chạy"

# 8. Kiểm tra file config
log "🔍 Kiểm tra file config của n8n..."
CONFIG_CONTENT=$(docker exec n8n_main cat /home/node/.n8n/config 2>/dev/null)
if [ $? -ne 0 ]; then
  log "⚠️ Không thể đọc file config, có thể file chưa được tạo"
else
  echo "$CONFIG_CONTENT"
  log "✅ Đã đọc file config thành công"
fi

# 9. Kiểm tra chú thích không mong muốn trong file config
log "🔍 Kiểm tra chú thích không mong muốn trong file config..."
if echo "$CONFIG_CONTENT" | grep -q "# <<<=== THAY BẰNG KHÓA MẠNH CỦA BẠN"; then
  log "⚠️ Tìm thấy chú thích không mong muốn trong file config"
  log "🔧 Đang sửa file config..."
  
  # Lấy giá trị N8N_ENCRYPTION_KEY từ file .env
  CLEAN_KEY=$(grep "N8N_ENCRYPTION_KEY" .env | cut -d '=' -f2 | xargs)
  
  # Kiểm tra xem CLEAN_KEY có giá trị không
  if [ -z "$CLEAN_KEY" ]; then
    handle_error "Không tìm thấy giá trị N8N_ENCRYPTION_KEY trong file .env"
  fi
  
  # Tạo file config tạm thời trên máy host
  log "📝 Tạo file config tạm thời..."
  echo "{\"encryptionKey\": \"$CLEAN_KEY\"}" > ./n8n_data/config.new
  
  # Sao chép file config vào container
  log "📂 Sao chép file config vào container..."
  docker cp ./n8n_data/config.new n8n_main:/home/node/.n8n/config
  
  # Khởi động lại n8n
  log "🔄 Khởi động lại n8n để áp dụng cấu hình mới..."
  docker restart n8n_main
  
  # Đợi n8n khởi động lại với thời gian dài hơn
  log "⏳ Đang đợi n8n khởi động lại (30 giây)..."
  sleep 30
  
  # Kiểm tra trạng thái container
  if ! docker ps | grep -q "n8n_main"; then
    log "⚠️ Container n8n_main không khởi động lại được, kiểm tra logs..."
    docker logs n8n_main --tail 50
    handle_error "Container n8n_main không khởi động lại được sau khi sửa config"
  fi
  
  # Kiểm tra lại file config
  log "🔍 Kiểm tra lại file config sau khi sửa..."
  NEW_CONFIG_CONTENT=$(docker exec n8n_main cat /home/node/.n8n/config 2>/dev/null)
  if [ $? -ne 0 ]; then
    handle_error "Không thể đọc file config sau khi sửa"
  fi
  echo "$NEW_CONFIG_CONTENT"
  
  # Kiểm tra lại chú thích
  if echo "$NEW_CONFIG_CONTENT" | grep -q "# <<<=== THAY BẰNG KHÓA MẠNH CỦA BẠN"; then
    handle_error "Vẫn còn chú thích không mong muốn trong file config sau khi sửa"
  else
    log "✅ Đã sửa file config thành công"
  fi
else
  log "✅ Không tìm thấy chú thích không mong muốn trong file config"
fi

# 10. Kiểm tra trạng thái các container
log "🔍 Kiểm tra trạng thái các container..."
docker-compose ps

# 11. Hiển thị URL truy cập n8n
log "🌐 n8n đã được triển khai thành công và có thể truy cập tại:"
echo "   http://localhost:5678"
if [ ! -z "$WEBHOOK_URL" ]; then
  echo "   hoặc: $WEBHOOK_URL"
fi

log "✅ TRIỂN KHAI N8N HOÀN TẤT THÀNH CÔNG"
exit 0