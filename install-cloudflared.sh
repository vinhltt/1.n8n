#!/bin/bash
# Tên: install-cloudflared.sh
# Mục đích: Cài đặt và cấu hình cloudflared

echo "===== Cài đặt Cloudflare Tunnel CLI (cloudflared) ====="

# Tải xuống phiên bản mới nhất của cloudflared
echo "1. Tải xuống cloudflared..."
curl -L --output cloudflared.deb https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb

# Cài đặt package
echo "2. Cài đặt cloudflared..."
sudo dpkg -i cloudflared.deb

# Kiểm tra cài đặt
echo "3. Kiểm tra phiên bản cloudflared..."
cloudflared version

# Xóa file tải xuống
echo "4. Dọn dẹp..."
rm cloudflared.deb

echo "===== Hướng dẫn thêm ====="
echo "1. Đăng nhập vào Cloudflare:"
echo "   cloudflared login"
echo ""
echo "2. File cert.pem sẽ được lưu tại ~/.cloudflared/"
echo ""
echo "3. Sao chép cert.pem vào thư mục cấu hình:"
echo "   mkdir -p ./cloudflared-config"
echo "   cp ~/.cloudflared/cert.pem ./cloudflared-config/"
echo ""
echo "4. Hoặc thay đổi cấu hình docker-compose để sử dụng token trực tiếp:"
echo "   command: tunnel --no-autoupdate run --token \${CLOUDFLARE_TUNNEL_TOKEN}"
echo ""
echo "5. Khởi động lại container:"
echo "   docker-compose down"
echo "   docker-compose up -d"
echo ""
echo "===== Cài đặt hoàn tất ====="