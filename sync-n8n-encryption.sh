#!/bin/bash
# sync-n8n-encryption.sh - Đồng bộ hóa khóa mã hóa n8n và khắc phục lỗi "Mismatching encryption keys"

echo "===== n8n Encryption Key Synchronization Tool ====="
echo "Script này sẽ đồng bộ hóa khóa mã hóa n8n giữa container, file cấu hình và GitHub Secrets"
echo ""

# Kiểm tra xem Docker đã chạy chưa
check_docker() {
  docker info &>/dev/null
  return $?
}

# Kiểm tra xem script có đang chạy trong GitHub Actions runner không
is_github_actions() {
  if [ -n "$GITHUB_ACTIONS" ] || [ -d "$HOME/actions-runner" ] || [[ "$PWD" == *"actions-runner"* ]]; then
    return 0  # Đang chạy trong GitHub Actions
  else
    return 1  # Không phải GitHub Actions
  fi
}

# Thử thực hiện lệnh với sudo nếu cần
run_with_sudo() {
  local cmd="$1"
  if check_docker; then
    # Docker hoạt động bình thường, chạy lệnh trực tiếp
    eval "$cmd"
  else
    # Docker không kết nối được, thử lại với sudo
    echo "Thử kết nối Docker với sudo..."
    sudo bash -c "$cmd"
  fi
}

# Kiểm tra xem script có đang chạy với quyền root không
if [ "$EUID" -ne 0 ] && ! check_docker && ! is_github_actions; then
  echo "Script này cần quyền root để truy cập Docker volumes."
  echo "Đang chạy lại với sudo..."
  sudo "$0" "$@"
  exit $?
fi

# Tìm khóa mã hóa từ file .env hiện tại (nếu có)
ENV_FILE="$(pwd)/.env"
CURRENT_ENV_KEY=""

if [ -f "$ENV_FILE" ]; then
  echo "Tìm khóa mã hóa trong file .env hiện tại..."
  CURRENT_ENV_KEY=$(grep "N8N_ENCRYPTION_KEY" "$ENV_FILE" | cut -d'=' -f2)
  if [ -n "$CURRENT_ENV_KEY" ]; then
    echo "✅ Đã tìm thấy khóa mã hóa trong file .env"
  else
    echo "❌ Không tìm thấy khóa mã hóa trong file .env"
  fi
fi

# Tìm khóa mã hóa từ container n8n
CONTAINER_KEY=""
if check_docker; then
  echo "Kiểm tra container n8n..."
  
  # Kiểm tra xem container n8n có đang chạy không
  if run_with_sudo "docker ps --filter 'name=n8n_main' --format '{{.Names}}' | grep -q 'n8n_main'"; then
    echo "✅ Tìm thấy container n8n đang chạy, trích xuất khóa mã hóa..."
    CONTAINER_KEY=$(run_with_sudo "docker exec n8n_main sh -c 'cat /home/node/.n8n/config 2>/dev/null | grep -o \"\\\"encryptionKey\\\": \\\"[^\\\"]*\\\"\" | cut -d\\\"\\\" -f4'")
    
    if [ -n "$CONTAINER_KEY" ]; then
      echo "✅ Đã trích xuất khóa mã hóa từ container"
    else
      echo "❌ Không thể trích xuất khóa mã hóa từ container"
    fi
  else
    echo "⚠️ Không tìm thấy container n8n đang chạy"
  fi
  
  # Nếu container không chạy hoặc không trích xuất được khóa, thử dừng và kiểm tra volume
  if [ -z "$CONTAINER_KEY" ]; then
    echo "Dừng container n8n để kiểm tra volume..."
    run_with_sudo "docker-compose down" || echo "⚠️ Không thể dừng container (có thể container đã dừng)"
    
    echo "Tìm đường dẫn đến volume n8n_data..."
    VOLUME_PATH=$(run_with_sudo "docker volume inspect n8n_data -f '{{.Mountpoint}}' 2>/dev/null")
    
    if [ -n "$VOLUME_PATH" ]; then
      echo "✅ Đã tìm thấy volume n8n_data tại: $VOLUME_PATH"
      
      # Tìm file cấu hình trong các vị trí phổ biến
      CONFIG_LOCATIONS=(
        "$VOLUME_PATH/config"
        "$VOLUME_PATH/.n8n/config"
        "$VOLUME_PATH/data/config"
      )
      
      CONFIG_FILE=""
      for loc in "${CONFIG_LOCATIONS[@]}"; do
        if run_with_sudo "test -f '$loc'"; then
          CONFIG_FILE="$loc"
          echo "✅ Đã tìm thấy file cấu hình tại: $CONFIG_FILE"
          break
        fi
      done
      
      # Tìm kiếm sâu hơn nếu cần
      if [ -z "$CONFIG_FILE" ]; then
        echo "Tìm kiếm sâu hơn trong volume..."
        FOUND_FILES=$(run_with_sudo "find '$VOLUME_PATH' -type f -name 'config' 2>/dev/null")
        
        if [ -n "$FOUND_FILES" ]; then
          CONFIG_FILE=$(echo "$FOUND_FILES" | head -n 1)
          echo "✅ Đã tìm thấy file cấu hình tại: $CONFIG_FILE"
        else
          echo "❌ Không tìm thấy file cấu hình trong volume"
        fi
      fi
      
      # Trích xuất khóa mã hóa từ file cấu hình
      if [ -n "$CONFIG_FILE" ]; then
        # Sửa quyền truy cập file config để khắc phục cảnh báo
        run_with_sudo "chmod 600 '$CONFIG_FILE'"
        echo "✅ Đã cập nhật quyền file cấu hình thành 600"
        
        # Trích xuất khóa mã hóa
        CONTAINER_KEY=$(run_with_sudo "grep -o '\"encryptionKey\": \"[^\"]*\"' '$CONFIG_FILE' | cut -d'\"' -f4")
        
        if [ -z "$CONTAINER_KEY" ]; then
          echo "Thử mẫu khác để trích xuất khóa..."
          CONTAINER_KEY=$(run_with_sudo "grep -o '\"encryptionKey\":\"[^\"]*\"' '$CONFIG_FILE' | cut -d'\"' -f4")
        fi
        
        if [ -n "$CONTAINER_KEY" ]; then
          echo "✅ Đã trích xuất khóa mã hóa từ file cấu hình"
        else
          echo "❌ Không thể trích xuất khóa mã hóa từ file cấu hình"
        fi
      fi
    else
      echo "⚠️ Không tìm thấy volume n8n_data (có thể Docker chưa sẵn sàng)"
    fi
  fi
else
  echo "⚠️ Docker không khả dụng hoặc không có quyền truy cập Docker"
fi

# Quyết định khóa mã hóa cuối cùng sẽ sử dụng
ENCRYPTION_KEY=""

# Ưu tiên 1: Khóa từ container/volume
if [ -n "$CONTAINER_KEY" ]; then
  ENCRYPTION_KEY="$CONTAINER_KEY"
  echo "ℹ️ Sử dụng khóa mã hóa từ container/volume"
# Ưu tiên 2: Khóa từ file .env hiện tại
elif [ -n "$CURRENT_ENV_KEY" ]; then
  ENCRYPTION_KEY="$CURRENT_ENV_KEY"
  echo "ℹ️ Sử dụng khóa mã hóa từ file .env hiện tại"
# Ưu tiên 3: Tạo khóa mới
else
  echo "Tạo khóa mã hóa mới..."
  ENCRYPTION_KEY=$(openssl rand -base64 24)
  echo "✅ Đã tạo khóa mã hóa mới"
fi

echo "Khóa mã hóa: $ENCRYPTION_KEY"

# Cập nhật file .env
echo "Cập nhật file .env với khóa mã hóa..."

# Tạo bản sao lưu file .env hiện tại
if [ -f "$ENV_FILE" ]; then
  cp "$ENV_FILE" "${ENV_FILE}.backup"
  echo "✅ Đã tạo bản sao lưu file .env tại ${ENV_FILE}.backup"
fi

# Cập nhật hoặc thêm N8N_ENCRYPTION_KEY vào file .env
if [ -f "$ENV_FILE" ]; then
  if grep -q "N8N_ENCRYPTION_KEY" "$ENV_FILE"; then
    # Thay thế dòng hiện có
    sed -i "s|N8N_ENCRYPTION_KEY=.*|N8N_ENCRYPTION_KEY=$ENCRYPTION_KEY|g" "$ENV_FILE"
  else
    # Thêm dòng mới
    echo "N8N_ENCRYPTION_KEY=$ENCRYPTION_KEY" >> "$ENV_FILE"
  fi
else
  # Tạo file .env mới
  echo "N8N_ENCRYPTION_KEY=$ENCRYPTION_KEY" > "$ENV_FILE"
fi

# Thêm cấu hình về quyền file settings
if grep -q "N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS" "$ENV_FILE"; then
  # Thay thế dòng hiện có
  sed -i "s|N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=.*|N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true|g" "$ENV_FILE"
else
  # Thêm dòng mới
  echo "N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true" >> "$ENV_FILE"
fi

echo "✅ Đã cập nhật file .env với khóa mã hóa và cấu hình quyền file"

# Cập nhật file cấu hình n8n nếu Docker đang hoạt động
if check_docker && [ -n "$CONFIG_FILE" ]; then
  echo "Cập nhật khóa mã hóa trong file cấu hình..."
  # Tạo bản sao lưu
  run_with_sudo "cp '$CONFIG_FILE' '${CONFIG_FILE}.backup'"
  
  # Kiểm tra xem khóa mã hóa đã tồn tại trong file chưa
  if run_with_sudo "grep -q 'encryptionKey' '$CONFIG_FILE'"; then
    # Thay thế khóa hiện có - xử lý các định dạng khác nhau
    run_with_sudo "sed -i 's|\"encryptionKey\": \"[^\"]*\"|\"encryptionKey\": \"$ENCRYPTION_KEY\"|g' '$CONFIG_FILE'"
    run_with_sudo "sed -i 's|\"encryptionKey\":\"[^\"]*\"|\"encryptionKey\":\"$ENCRYPTION_KEY\"|g' '$CONFIG_FILE'"
  else
    # Thêm khóa mới vào file JSON (cần xử lý cẩn thận để không phá vỡ định dạng JSON)
    run_with_sudo "sed -i 's|}|, \"encryptionKey\": \"$ENCRYPTION_KEY\"\\n}|' '$CONFIG_FILE'"
  fi
  
  echo "✅ Đã cập nhật khóa mã hóa trong file cấu hình"
fi

# Lưu khóa mã hóa cho GitHub Secrets
echo ""
echo "Đây là khóa mã hóa để cập nhật vào GitHub Secrets (N8N_ENCRYPTION_KEY):"
echo ""
echo "$ENCRYPTION_KEY"
echo ""
echo "=== QUAN TRỌNG ==="
echo "1. Hãy cập nhật GitHub Secret 'N8N_ENCRYPTION_KEY' với giá trị trên."
echo "2. Thêm 'N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true' vào file .env và cấu hình Docker"
echo ""

# Khởi động lại n8n nếu không phải đang chạy trong GitHub Actions
if ! is_github_actions; then
  echo "Bạn có muốn khởi động lại n8n ngay bây giờ không? (y/n)"
  read -r RESTART

  if [[ $RESTART == "y" || $RESTART == "Y" ]]; then
    echo "Khởi động lại n8n..."
    run_with_sudo "docker-compose up -d"
    echo "✅ Đã khởi động lại n8n."
    echo "Kiểm tra log để đảm bảo không còn lỗi:"
    echo "docker logs n8n_main"
  else
    echo "Bạn có thể khởi động lại n8n thủ công bằng lệnh:"
    echo "docker-compose up -d"
  fi
else
  echo "Đang chạy trong GitHub Actions runner, bỏ qua phần khởi động lại n8n."
  echo "Giá trị khóa mã hóa sẽ được sử dụng trong quy trình triển khai."
fi

echo ""
echo "===== Đồng bộ hóa hoàn tất ====="