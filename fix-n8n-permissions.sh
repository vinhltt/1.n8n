#!/bin/bash
# Kiểm tra và sửa quyền truy cập file n8n

echo "Đang kiểm tra quyền truy cập file cho n8n..."

# Kiểm tra xem container có tồn tại không
if docker ps -a | grep -q "n8n_main"; then
  echo "Tìm thấy container n8n. Đang sửa quyền truy cập file..."

  # Chạy lệnh để cấp quyền cho thư mục n8n
  docker exec -u root n8n_main sh -c "chown -R node:node /home/node/.n8n && chmod -R 755 /home/node/.n8n"
  
  # Kiểm tra kết quả
  if [ $? -eq 0 ]; then
    echo "✅ Đã sửa quyền truy cập file thành công!"
  else
    echo "❌ Không thể sửa quyền truy cập file. Vui lòng kiểm tra lỗi."
  fi
else
  echo "❌ Không tìm thấy container n8n. Vui lòng đảm bảo container đang chạy."
fi

echo "Hoàn tất!"