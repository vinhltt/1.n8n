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
  # Thử lấy tên service postgres từ docker-compose.yml (ưu tiên postgresdb)
  if command -v docker-compose &>/dev/null; then
    POSTGRES_SERVICE=$(docker-compose ps --services | grep -E '^postgres(db)?$' | head -n1)
    # Lấy container ID từ service
    POSTGRES_CONTAINER_ID=$(docker-compose ps -q "$POSTGRES_SERVICE")
  else
    POSTGRES_SERVICE=$(docker compose ps --services | grep -E '^postgres(db)?$' | head -n1)
    POSTGRES_CONTAINER_ID=$(docker compose ps -q "$POSTGRES_SERVICE")
  fi
  if [ -z "$POSTGRES_CONTAINER_ID" ]; then
    echo "Lỗi: Không xác định được container PostgreSQL từ docker-compose." >&2; exit 1;
  fi
  # Lấy tên container thực tế từ container ID
  POSTGRES_CONTAINER=$(docker ps --filter id="$POSTGRES_CONTAINER_ID" --format '{{.Names}}')
  if [ -z "$POSTGRES_CONTAINER" ]; then
    echo "Lỗi: Không lấy được tên container PostgreSQL từ ID." >&2; exit 1;
  fi
fi
POSTGRES_CONTAINER=${POSTGRES_CONTAINER}
DB_NAME=${POSTGRES_DB:-n8n_database}
if [ -z "${N8N_VOLUME_NAME:-}" ]; then
  # Lấy prefix từ tên container PostgreSQL, ví dụ: pfm_prod-postgresdb-1
  PREFIX=$(echo "$POSTGRES_CONTAINER" | sed -E 's/-postgres(db)?-[0-9]+$//')
  N8N_VOLUME_NAME="${PREFIX}_n8n_data"
fi
RETENTION_DAYS=${N8N_RETENTION_DAYS:-7}

: "${POSTGRES_USER:?Lỗi: POSTGRES_USER chưa đặt}"
: "${POSTGRES_PASSWORD:?Lỗi: POSTGRES_PASSWORD chưa đặt}"
: "${N8N_BACKUP_DIR_HOST:?Lỗi: N8N_BACKUP_DIR_HOST chưa đặt}"

DB_USER="$POSTGRES_USER"
DB_PASSWORD="$POSTGRES_PASSWORD"
BACKUP_DIR="$N8N_BACKUP_DIR_HOST" # Thay đổi đường dẫn backup

# 4) Sanity checks Docker
docker inspect "$POSTGRES_CONTAINER" >/dev/null 2>&1 \
  || { echo "Lỗi: container $POSTGRES_CONTAINER không tồn tại." >&2; exit 1; }
docker volume inspect "$N8N_VOLUME_NAME" >/dev/null 2>&1 \
  || { echo "Lỗi: volume $N8N_VOLUME_NAME không tồn tại." >&2; exit 1; }

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
