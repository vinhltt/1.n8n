#!/bin/bash
# Kiểm tra và sửa quyền truy cập file n8n

echo "Đang kiểm tra quyền truy cập file cho n8n..."

# Kiểm tra xem container có tồn tại và đang chạy không
if docker ps | grep -q "n8n_main"; then
  echo "Tìm thấy container n8n đang chạy. Đang sửa quyền truy cập file..."

  # Chạy lệnh để cấp quyền cho thư mục n8n
  docker exec -u root n8n_main sh -c "chown -R node:node /home/node/.n8n && chmod -R 755 /home/node/.n8n"
  
  # Kiểm tra kết quả
  if [ $? -eq 0 ]; then
    echo "✅ Đã sửa quyền truy cập file thành công!"
  else
    echo "❌ Không thể sửa quyền truy cập file. Vui lòng kiểm tra lỗi."
  fi
else
  # Kiểm tra xem container có tồn tại nhưng không chạy
  if docker ps -a | grep -q "n8n_main"; then
    echo "⚠️ Container n8n_main tồn tại nhưng không chạy. Sẽ sửa quyền trong volume trực tiếp..."
    
    # Sử dụng container tạm để sửa quyền trong volume
    docker run --rm -v n8n_data:/data alpine sh -c "mkdir -p /data/.n8n && chmod -R 755 /data/.n8n && echo '✅ Đã sửa quyền thư mục trong volume'"
    
    echo "✅ Đã sửa quyền trong volume. Quyền sẽ được áp dụng khi container khởi động."
  else
    echo "❌ Không tìm thấy container n8n. Đang thử sửa quyền trên volume..."
    
    # Thử sửa quyền trong volume mà không cần container
    if docker volume inspect n8n_data > /dev/null 2>&1; then
      docker run --rm -v n8n_data:/data alpine sh -c "mkdir -p /data/.n8n && chmod -R 755 /data/.n8n && echo '✅ Đã sửa quyền thư mục trong volume'"
      echo "✅ Đã sửa quyền trong volume n8n_data."
    else
      echo "❌ Không tìm thấy volume n8n_data. Không thể sửa quyền."
    fi
  fi
fi

echo "Hoàn tất!"