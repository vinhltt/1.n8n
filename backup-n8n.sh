#!/usr/bin/env bash
# =====================================================================
#  n8n + PostgreSQL backup script
# =====================================================================

set -euo pipefail
set -x
IFS=$'\n\t'

# 1) Đảm bảo đang dùng bash
if [ -z "${BASH_VERSION:-}" ]; then
  echo "Error: Script phải chạy với bash, không phải sh." >&2
  exit 1
fi

# 2) Xác định thư mục chứa script & load .env (lọc CRLF nếu có)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="$SCRIPT_DIR/.env"
[[ -f "$ENV_FILE" ]] || { echo "Lỗi: Không tìm thấy .env tại $ENV_FILE" >&2; exit 1; }
set -a
source <(sed 's/\r$//' "$ENV_FILE")
set +a

# 3) Biến cấu hình, báo lỗi nếu thiếu
# --- Lấy tên container PostgreSQL thực tế ---
if [ -z "${POSTGRES_CONTAINER:-}" ]; then
  # Ưu tiên sử dụng COMPOSE_PROJECT_NAME để construct container name
  if [ -n "${COMPOSE_PROJECT_NAME:-}" ]; then
    POSTGRES_CONTAINER="${COMPOSE_PROJECT_NAME}-postgresdb-1"
    echo "🔍 Using COMPOSE_PROJECT_NAME to construct container: $POSTGRES_CONTAINER"
  else
    # Fallback: Thử lấy từ docker-compose nếu không có COMPOSE_PROJECT_NAME
    echo "🔍 COMPOSE_PROJECT_NAME not set, trying auto-detection..."
    if command -v docker-compose &>/dev/null; then
      POSTGRES_SERVICE=$(docker-compose ps --services 2>/dev/null | grep -E '^postgres(db)?$' | head -n1)
      if [ -n "$POSTGRES_SERVICE" ]; then
        POSTGRES_CONTAINER_ID=$(docker-compose ps -q "$POSTGRES_SERVICE" 2>/dev/null)
      fi
    else
      POSTGRES_SERVICE=$(docker compose ps --services 2>/dev/null | grep -E '^postgres(db)?$' | head -n1)
      if [ -n "$POSTGRES_SERVICE" ]; then
        POSTGRES_CONTAINER_ID=$(docker compose ps -q "$POSTGRES_SERVICE" 2>/dev/null)
      fi
    fi
    
    if [ -n "$POSTGRES_CONTAINER_ID" ]; then
      POSTGRES_CONTAINER=$(docker ps --filter id="$POSTGRES_CONTAINER_ID" --format '{{.Names}}')
    fi
    
    if [ -z "$POSTGRES_CONTAINER" ]; then
      echo "❌ Không thể xác định container PostgreSQL."
      echo "   Hãy đặt biến POSTGRES_CONTAINER hoặc COMPOSE_PROJECT_NAME"
      echo "   Các container đang chạy:"
      docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Image}}"
      exit 1
    fi
  fi
fi

DB_NAME=${POSTGRES_DB:-n8n_database}
if [ -z "${N8N_VOLUME_NAME:-}" ]; then
  if [ -n "${COMPOSE_PROJECT_NAME:-}" ]; then
    N8N_VOLUME_NAME="${COMPOSE_PROJECT_NAME}_n8n_data"
    echo "🔍 Using COMPOSE_PROJECT_NAME to construct volume: $N8N_VOLUME_NAME"
  else
    # Fallback: Lấy prefix từ tên container PostgreSQL
    PREFIX=$(echo "$POSTGRES_CONTAINER" | sed -E 's/-postgres(db)?-[0-9]+$//')
    N8N_VOLUME_NAME="${PREFIX}_n8n_data"
    echo "🔍 Extracted prefix '$PREFIX' from container name"
  fi
fi
RETENTION_DAYS=${N8N_RETENTION_DAYS:-7}

: "${POSTGRES_USER:?Lỗi: POSTGRES_USER chưa đặt}"
: "${POSTGRES_PASSWORD:?Lỗi: POSTGRES_PASSWORD chưa đặt}"
: "${N8N_BACKUP_DIR_HOST:?Lỗi: N8N_BACKUP_DIR_HOST chưa đặt}"

DB_USER="$POSTGRES_USER"
DB_PASSWORD="$POSTGRES_PASSWORD"
BACKUP_DIR="$N8N_BACKUP_DIR_HOST" # Thay đổi đường dẫn backup

# 4) Sanity checks Docker với debugging
echo "🔍 Kiểm tra container và volume..."
echo "   Container cần backup: $POSTGRES_CONTAINER"
echo "   Volume cần backup: $N8N_VOLUME_NAME"

# List containers for debugging
echo "   Các container đang chạy:"
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Image}}" | head -10

if ! docker inspect "$POSTGRES_CONTAINER" >/dev/null 2>&1; then
  echo "❌ Container $POSTGRES_CONTAINER không tồn tại hoặc không chạy."
  echo "   Các container hiện có:"
  docker ps -a --format "table {{.Names}}\t{{.Status}}\t{{.Image}}"
  exit 1
fi

# List volumes for debugging  
echo "   Các volume hiện có:"
docker volume ls | head -10

if ! docker volume inspect "$N8N_VOLUME_NAME" >/dev/null 2>&1; then
  echo "❌ Volume $N8N_VOLUME_NAME không tồn tại."
  echo "   Các volume hiện có:"
  docker volume ls
  exit 1
fi

echo "✅ Container và volume đã được xác nhận."

# 5) Tạo RUN_DIR & file log trong đó
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
RUN_DIR="$BACKUP_DIR/$TIMESTAMP"
mkdir -p "$RUN_DIR"

LOG_FILE="$RUN_DIR/backup.log"

# Redirect stdout & stderr của toàn bộ script vào cả console và file log
exec > >(tee -a "$LOG_FILE") 2>&1

echo "=================================================================="
echo "BẮT ĐẦU backup lúc:  $(date '+%F %T')"
echo "RUN_DIR:            $RUN_DIR"
echo "Container DB:       $POSTGRES_CONTAINER (DB: $DB_NAME)"
echo "Volume:             $N8N_VOLUME_NAME"
echo "Giữ lại:            $RETENTION_DAYS ngày"
echo "------------------------------------------------------------------"

FAILED=0

# 6.1) Dump PostgreSQL
DB_FILE="n8n_db_backup.sql.gz"
echo "[*] Dumping PostgreSQL …"
if docker exec -i \
     -e PGPASSWORD="$DB_PASSWORD" \
     "$POSTGRES_CONTAINER" \
     pg_dump -U "$DB_USER" -d "$DB_NAME" -Fc 2>/dev/null \
     | gzip > "$RUN_DIR/$DB_FILE"; then
  echo "    → OK: $DB_FILE"
else
  echo "    → LỖI: backup database thất bại."
  FAILED=1
fi

# 6.2) Archive Docker volume
VOL_FILE="n8n_data_backup.tar.gz"
echo "[*] Archiving Docker volume …"
if docker run --rm \
     -v "${N8N_VOLUME_NAME}:/volume_data:ro" \
     -v "${RUN_DIR}:/backup_target" \
     alpine sh -c "tar czf \"/backup_target/$VOL_FILE\" -C /volume_data ."; then
  echo "    → OK: $VOL_FILE"
else
  echo "    → LỖI: backup volume thất bại."
  FAILED=1
fi

# 6.3) Cleanup các RUN_DIR cũ
echo "[*] Xoá thư mục backup cũ (> $RETENTION_DAYS ngày) …"
find "$BACKUP_DIR" -maxdepth 1 -mindepth 1 -type d \
     -name '[0-9]*_[0-9]*' -mtime +"$RETENTION_DAYS" \
     -print -exec rm -rf {} +

echo "------------------------------------------------------------------"
if [[ "$FAILED" -eq 0 ]]; then
  echo "✅  Backup hoàn tất thành công."
else
  echo "❌  Backup hoàn tất nhưng CÓ LỖI (xem log)."
fi
echo "KẾT THÚC lúc:  $(date '+%F %T')"
echo "=================================================================="

exit $FAILED
