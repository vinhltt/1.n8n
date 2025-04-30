#!/bin/bash
# deploy-n8n.sh - Script triển khai n8n

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

# 1. Pull images mới nhất
log "🔄 Đang pull images mới nhất..."
docker-compose pull || handle_error "Không thể pull images"
log "✅ Đã pull images thành công"

# 2. Khởi động container
log "🚀 Đang khởi động containers..."
docker-compose up -d || handle_error "Không thể khởi động containers"
log "✅ Đã khởi động containers thành công"

# 3. Đợi n8n khởi động
log "⏳ Đang đợi n8n khởi động..."
sleep 15
log "✅ Đã đợi đủ thời gian cho n8n khởi động"

# 4. Kiểm tra file config
log "🔍 Kiểm tra file config của n8n..."
if ! docker exec n8n_main cat /home/node/.n8n/config; then
  log "⚠️ Không thể đọc file config"
else
  log "✅ Đã đọc file config thành công"
fi

# 5. Kiểm tra chú thích không mong muốn trong file config
log "🔍 Kiểm tra chú thích không mong muốn trong file config..."
if docker exec n8n_main sh -c "grep -q '# <<<=== THAY BẰNG KHÓA MẠNH CỦA BẠN' /home/node/.n8n/config"; then
  log "⚠️ Tìm thấy chú thích không mong muốn trong file config"
  log "🔧 Đang sửa file config..."
  
  # Lấy giá trị N8N_ENCRYPTION_KEY từ file .env
  CLEAN_KEY=$(grep "N8N_ENCRYPTION_KEY" .env | cut -d '=' -f2 | xargs)
  
  # Kiểm tra xem CLEAN_KEY có giá trị không
  if [ -z "$CLEAN_KEY" ]; then
    handle_error "Không tìm thấy giá trị N8N_ENCRYPTION_KEY trong file .env"
  fi
  
  # Tạo file config mới không có chú thích
  if ! docker exec n8n_main sh -c "echo '{\"encryptionKey\": \"$CLEAN_KEY\"}' > /home/node/.n8n/config"; then
    handle_error "Không thể cập nhật file config"
  fi
  
  # Khởi động lại n8n
  log "🔄 Khởi động lại n8n để áp dụng cấu hình mới..."
  if ! docker restart n8n_main; then
    handle_error "Không thể khởi động lại n8n"
  fi
  
  # Đợi n8n khởi động lại
  log "⏳ Đang đợi n8n khởi động lại..."
  sleep 10
  
  # Kiểm tra lại file config
  log "🔍 Kiểm tra lại file config sau khi sửa..."
  if ! docker exec n8n_main cat /home/node/.n8n/config; then
    handle_error "Không thể đọc file config sau khi sửa"
  fi
  
  # Kiểm tra lại chú thích
  if docker exec n8n_main sh -c "grep -q '# <<<=== THAY BẰNG KHÓA MẠNH CỦA BẠN' /home/node/.n8n/config"; then
    handle_error "Vẫn còn chú thích không mong muốn trong file config sau khi sửa"
  else
    log "✅ Đã sửa file config thành công"
  fi
else
  log "✅ Không tìm thấy chú thích không mong muốn trong file config"
fi

# 6. Kiểm tra trạng thái các container
log "🔍 Kiểm tra trạng thái các container..."
docker-compose ps

# 7. Kiểm tra log của n8n (tùy chọn)
log "🔍 Hiển thị log của n8n (10 dòng gần nhất)..."
docker logs --tail 10 n8n_main

log "✅ TRIỂN KHAI N8N HOÀN TẤT THÀNH CÔNG"
exit 0