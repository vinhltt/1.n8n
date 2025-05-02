#!/bin/bash
# check-runner-cache.sh - Kiểm tra các vị trí cache trên GitHub Action Runner

echo "===== KIỂM TRA CACHE TRÊN GITHUB ACTION RUNNER ====="

echo -e "\n1. Kiểm tra file .env trong thư mục làm việc hiện tại:"
if [ -f .env ]; then
  echo "  ⚠️ Tìm thấy file .env"
  echo "  Nội dung của biến N8N_ENCRYPTION_KEY trong .env:"
  grep "N8N_ENCRYPTION_KEY" .env || echo "  Không tìm thấy biến N8N_ENCRYPTION_KEY trong .env"
else
  echo "  ✅ Không tìm thấy file .env"
fi

echo -e "\n2. Kiểm tra các file .env ẩn trong thư mục làm việc:"
find . -name "*.env*" -type f 2>/dev/null | while read file; do
  echo "  ⚠️ Tìm thấy file .env ẩn: $file"
  echo "  Nội dung của biến N8N_ENCRYPTION_KEY trong $file:"
  grep "N8N_ENCRYPTION_KEY" "$file" || echo "  Không tìm thấy biến N8N_ENCRYPTION_KEY trong $file"
done

echo -e "\n3. Kiểm tra file config trong thư mục n8n_data:"
if [ -f ./n8n_data/config ]; then
  echo "  ⚠️ Tìm thấy file config trong thư mục n8n_data"
  echo "  Nội dung file config:"
  cat ./n8n_data/config
else
  echo "  ✅ Không tìm thấy file config trong thư mục n8n_data"
fi

echo -e "\n4. Kiểm tra file cấu hình trong thư mục home của runner:"
HOME_ENV="$HOME/.env"
if [ -f "$HOME_ENV" ]; then
  echo "  ⚠️ Tìm thấy file .env trong thư mục home của runner"
  echo "  Nội dung của biến N8N_ENCRYPTION_KEY trong $HOME_ENV:"
  grep "N8N_ENCRYPTION_KEY" "$HOME_ENV" || echo "  Không tìm thấy biến N8N_ENCRYPTION_KEY trong $HOME_ENV"
else
  echo "  ✅ Không tìm thấy file .env trong thư mục home của runner"
fi

echo -e "\n5. Kiểm tra file docker-compose.override.yml:"
if [ -f docker-compose.override.yml ]; then
  echo "  Nội dung liên quan đến N8N_ENCRYPTION_KEY trong docker-compose.override.yml:"
  grep -A 2 -B 2 "N8N_ENCRYPTION_KEY" docker-compose.override.yml || echo "  Không tìm thấy N8N_ENCRYPTION_KEY trong docker-compose.override.yml"
fi

echo -e "\n6. Kiểm tra file .github/workflows:"
if [ -d .github/workflows ]; then
  echo "  Nội dung liên quan đến N8N_ENCRYPTION_KEY trong các file workflow:"
  grep -r "N8N_ENCRYPTION_KEY" .github/workflows/ || echo "  Không tìm thấy N8N_ENCRYPTION_KEY trong các file workflow"
fi

echo -e "\n7. Kiểm tra n8n_data directory cho các file ẩn:"
if [ -d ./n8n_data ]; then
  echo "  Các file trong thư mục n8n_data (bao gồm các file ẩn):"
  ls -la ./n8n_data/
else 
  echo "  ✅ Không tìm thấy thư mục n8n_data"
fi

echo -e "\n8. Kiểm tra các file cấu hình Docker:"
echo "  Docker volumes:"
docker volume ls

echo -e "\n9. Kiểm tra biến môi trường trong runner:"
echo "  Các biến môi trường có chứa 'N8N', 'ENCRYPTION' hoặc 'n8n':"
env | grep -i "N8N\|ENCRYPTION\|n8n" || echo "  Không tìm thấy biến môi trường liên quan"

echo "===== KẾT THÚC KIỂM TRA ====="