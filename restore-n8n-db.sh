#!/bin/bash
# restore-n8n-db.sh - Script khôi phục database từ backup

# Hiển thị banner
echo "=============================================="
echo "    KHÔI PHỤC DATABASE n8n TỪ BACKUP FILE    "
echo "=============================================="

# Đặt các biến - cập nhật các giá trị này theo môi trường của bạn
BACKUP_FILE="$HOME/....."
PG_CONTAINER="postgresdb"
PG_USER="admin"
PG_PASSWORD="xxxxxx"  # Thay thế bằng mật khẩu thực của bạn
DB_NAME="n8n_database"

# Ghi log với timestamp
log() {
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
}

# Xử lý lỗi
handle_error() {
  log "❌ Lỗi: $1"
  exit 1
}

# 1. Sao chép file backup vào container
log "📂 Sao chép file backup vào container PostgreSQL..."
docker cp "$BACKUP_FILE" $PG_CONTAINER:/tmp/n8n_restore_db.sql.gz || handle_error "Không thể sao chép file backup"
log "✅ Đã sao chép file backup thành công"

# 2. Thực hiện khôi phục database
log "🔄 Khôi phục database từ backup file..."
docker exec -i -e PGPASSWORD="$PG_PASSWORD" $PG_CONTAINER bash -c '
    # Kiểm tra xem file đã giải nén chưa
    if [ -f "/tmp/n8n_restore_db.sql" ]; then
        echo "--- File đã được giải nén trước đó, xóa để giải nén lại ---"
        rm -f /tmp/n8n_restore_db.sql
    fi
    
    echo "--- Decompressing backup file ---"
    gunzip -f /tmp/n8n_restore_db.sql.gz || exit 1
    
    echo "--- Dropping existing database ('$DB_NAME') if exists ---"
    dropdb -U '$PG_USER' --if-exists '$DB_NAME' || exit 2
    
    echo "--- Creating new database ('$DB_NAME') ---"
    createdb -U '$PG_USER' '$DB_NAME' || exit 3
    
    echo "--- Detecting backup format ---"
    if pg_restore --list /tmp/n8n_restore_db.sql &> /dev/null; then
        echo "--- Detected PostgreSQL custom format, using pg_restore ---"
        pg_restore -U '$PG_USER' -d '$DB_NAME' /tmp/n8n_restore_db.sql || exit 4
    else
        echo "--- Detected plain SQL format, using psql ---"
        psql -U '$PG_USER' -d '$DB_NAME' < /tmp/n8n_restore_db.sql || exit 4
    fi
    
    echo "--- Cleaning up temporary file ---"
    rm -f /tmp/n8n_restore_db.sql
    
    echo "--- Database restore completed ---"
' || handle_error "Khôi phục database thất bại với mã lỗi $?"

log "✅ KHÔI PHỤC DATABASE HOÀN TẤT THÀNH CÔNG"
echo "=============================================="
echo "🚀 Đã khôi phục database thành công! Hãy tiếp tục với các bước tiếp theo để khôi phục dữ liệu n8n."