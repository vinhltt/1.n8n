#!/bin/bash
# restore-n8n-data.sh - Script khôi phục dữ liệu n8n từ file backup

# Hiển thị banner
echo "=============================================="
echo "     KHÔI PHỤC DỮ LIỆU n8n TỪ BACKUP FILE    "
echo "=============================================="

# Đặt các biến - cập nhật các giá trị này theo môi trường của bạn
BACKUP_FILE="$HOME/SynologyDrive/1.n8n/20250428_071502/n8n_data_backup_20250427_071501.tar.gz"
DOCKER_VOLUME="n8n_data"
TEMP_DIR="/tmp/n8n_data_restore_temp"

# Ghi log với timestamp
log() {
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
}

# Xử lý lỗi
handle_error() {
  log "❌ Lỗi: $1"
  exit 1
}

# 1. Kiểm tra file backup tồn tại
log "🔍 Kiểm tra file backup..."
if [ ! -f "$BACKUP_FILE" ]; then
  handle_error "File backup không tồn tại tại đường dẫn $BACKUP_FILE"
fi
log "✅ File backup tồn tại: $BACKUP_FILE"

# 2. Tạo thư mục tạm trên host
log "📁 Tạo thư mục tạm để giải nén backup..."
mkdir -p "$TEMP_DIR"
log "✅ Đã tạo thư mục tạm: $TEMP_DIR"

# 3. Giải nén file backup vào thư mục tạm
log "📦 Giải nén backup vào thư mục tạm..."
tar -xzf "$BACKUP_FILE" -C "$TEMP_DIR" || handle_error "Không thể giải nén file backup"
log "✅ Đã giải nén backup thành công"

# 4. Đếm số lượng file trong thư mục tạm
FILE_COUNT=$(find "$TEMP_DIR" -type f | wc -l)
log "📊 Số lượng file đã giải nén: $FILE_COUNT"

# 5. Xóa dữ liệu hiện tại trong volume và sao chép dữ liệu mới
log "🔄 Xóa dữ liệu hiện tại và khôi phục từ backup..."

# Sử dụng alpine image trong Docker để thực hiện xóa và khôi phục
docker run --rm \
  -v "$DOCKER_VOLUME":/volume_data \
  -v "$TEMP_DIR":/backup_data \
  alpine \
  sh -c ' \
    echo "--- Xóa dữ liệu hiện tại trong volume ---" && \
    rm -rf /volume_data/* && \
    echo "--- Khôi phục dữ liệu từ backup ---" && \
    cp -rf /backup_data/* /volume_data/ && \
    echo "--- Kiểm tra quyền truy cập file ---" && \
    chown -R 1000:1000 /volume_data/ && \
    echo "--- Hoàn tất khôi phục volume ---" \
  ' || handle_error "Không thể khôi phục dữ liệu vào volume Docker"

log "✅ Đã khôi phục dữ liệu vào volume $DOCKER_VOLUME thành công"

# 6. Xóa thư mục tạm
log "🧹 Dọn dẹp thư mục tạm..."
rm -rf "$TEMP_DIR"
log "✅ Đã xóa thư mục tạm"

log "✅ KHÔI PHỤC DỮ LIỆU HOÀN TẤT THÀNH CÔNG"
echo "=============================================="
echo "🚀 Đã khôi phục dữ liệu n8n thành công! Bây giờ bạn có thể khởi động lại container n8n"
echo ""
echo "Để khởi động lại n8n, chạy lệnh:"
echo "docker start n8n_main"
echo "hoặc"
echo "docker-compose up -d n8n"