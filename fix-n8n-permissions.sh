#!/bin/bash
# fix-n8n-permissions.sh - Fix permission problems for n8n config file

echo "===== n8n Permissions Fix Tool ====="
echo "Script này sẽ cấu hình n8n để sử dụng quyền file cấu hình đúng và áp dụng cài đặt N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true"
echo ""

# Kiểm tra xem Docker đã chạy chưa
check_docker() {
  docker info &>/dev/null
  return $?
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

# Kiểm tra xem script có đang chạy trong GitHub Actions runner không
is_github_actions() {
  if [ -n "$GITHUB_ACTIONS" ] || [ -d "$HOME/actions-runner" ] || [[ "$PWD" == *"actions-runner"* ]]; then
    return 0  # Đang chạy trong GitHub Actions
  else
    return 1  # Không phải GitHub Actions
  fi
}

# Kiểm tra xem container n8n có đang chạy không
if check_docker; then
  echo "Kiểm tra container n8n..."
  
  if run_with_sudo "docker ps --filter 'name=n8n_main' --format '{{.Names}}' | grep -q 'n8n_main'"; then
    echo "✅ Tìm thấy container n8n đang chạy"
    
    # Cập nhật quyền truy cập file cấu hình trong container
    echo "Cập nhật quyền truy cập file cấu hình trong container..."
    run_with_sudo "docker exec n8n_main sh -c 'chmod 600 /home/node/.n8n/config 2>/dev/null'"
    echo "✅ Đã cập nhật quyền file cấu hình thành 600"
  else
    echo "⚠️ Không tìm thấy container n8n đang chạy, sẽ áp dụng cấu hình khi container khởi động"
  fi
else
  echo "⚠️ Docker không khả dụng hoặc không có quyền truy cập Docker"
  echo "Sẽ áp dụng cài đặt quyền file thông qua biến môi trường"
fi

# Cập nhật file .env với cài đặt N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true
ENV_FILE="$(pwd)/.env"

# Tìm xem file .env đã tồn tại chưa
if [ -f "$ENV_FILE" ]; then
  echo "Tìm thấy file .env, cập nhật cài đặt quyền file..."
  
  # Kiểm tra xem cài đặt đã tồn tại chưa
  if grep -q "N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS" "$ENV_FILE"; then
    # Thay thế dòng hiện có
    sed -i "s|N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=.*|N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true|g" "$ENV_FILE"
  else
    # Thêm dòng mới
    echo "N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true" >> "$ENV_FILE"
  fi
  
  echo "✅ Đã cập nhật file .env với N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true"
else
  # Tạo file .env mới với cài đặt cơ bản
  echo "Không tìm thấy file .env, tạo file mới với cài đặt cơ bản..."
  echo "N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true" > "$ENV_FILE"
  echo "✅ Đã tạo file .env mới với cài đặt quyền file"
fi

echo ""
echo "=== QUAN TRỌNG ==="
echo "1. Đã áp dụng cài đặt N8N_ENFORCE_SETTINGS_FILE_PERMISSIONS=true trong file .env"
echo "2. Hãy đảm bảo cài đặt này được đưa vào file Docker Compose của bạn"
echo ""

# Nếu không phải đang chạy trong GitHub Actions, hỏi người dùng về việc khởi động lại
if ! is_github_actions; then
  echo "Bạn có muốn khởi động lại n8n ngay bây giờ không? (y/n)"
  read -r RESTART

  if [[ $RESTART == "y" || $RESTART == "Y" ]]; then
    echo "Khởi động lại n8n..."
    run_with_sudo "docker-compose down && docker-compose up -d"
    echo "✅ Đã khởi động lại n8n."
    echo "Kiểm tra log để đảm bảo không còn cảnh báo về quyền file:"
    echo "docker logs n8n_main"
  else
    echo "Bạn có thể khởi động lại n8n thủ công bằng lệnh:"
    echo "docker-compose down && docker-compose up -d"
  fi
else
  echo "Đang chạy trong GitHub Actions runner, bỏ qua phần khởi động lại n8n."
fi

echo ""
echo "===== Cấu hình quyền file hoàn tất ====="