# Hướng dẫn sử dụng GitHub Self-Hosted Runner cho triển khai n8n

File này hướng dẫn cách thiết lập GitHub Self-hosted Runner trên server để tự động triển khai n8n khi có thay đổi trong các file docker-compose hoặc khi kích hoạt thủ công.

## Ưu điểm của Self-hosted Runner

- **Không cần SSH qua Cloudflare Tunnel**: Tránh các vấn đề về kết nối SSH
- **Triển khai nhanh hơn**: Runner chạy trực tiếp trên server, không cần sao chép file
- **Bảo mật cao**: Không cần mở port SSH hoặc lưu trữ SSH key trên GitHub
- **Đơn giản hóa quy trình**: Loại bỏ các bước phức tạp để thiết lập SSH

## Cách thiết lập Self-hosted Runner

1. **Đăng nhập vào GitHub repository**:
  - Truy cập repository của bạn trên GitHub
  - Đi đến Settings > Actions > Runners
  - Nhấp vào "New self-hosted runner"

2. **Chọn môi trường**:
  - Chọn "Linux" làm Runner image
  - Làm theo hướng dẫn để tải xuống và cài đặt runner

3. **Thực thi script cài đặt trên server**:
  ```bash title="Cài đặt GitHub Runner"
  # Tải xuống
  mkdir actions-runner && cd actions-runner
  curl -o actions-runner-linux-x64-2.311.0.tar.gz -L https://github.com/actions/runner/releases/download/v2.311.0/actions-runner-linux-x64-2.311.0.tar.gz
  
  # Giải nén
  tar xzf ./actions-runner-linux-x64-2.311.0.tar.gz
  
  # Cấu hình
  ./config.sh --url https://github.com/yourusername/yourrepo --token ABCDEFGH12345
  
  # Cài đặt và khởi động service
  sudo ./svc.sh install
  sudo ./svc.sh start
  ```

4. **Đảm bảo runner có quyền chạy Docker**:
  ```bash title="Cấp quyền Docker"
  # Thêm user vào nhóm docker
  sudo usermod -aG docker $USER
  
  # Kiểm tra cài đặt
  docker info
  ```

## Cập nhật workflow file

Sau khi thiết lập runner, cập nhật file GitHub Actions workflow để sử dụng self-hosted runner:

```yaml title="GitHub Actions Workflow"
name: Deploy n8n

on:
  push:
    branches: [ develop ]
    paths:
      - 'docker-compose.yml'
      - 'docker-compose.override.yml'
      - 'cloudflared-config/**'
  workflow_dispatch:

jobs:
  deploy:
    runs-on: self-hosted  # Chỉ định sử dụng self-hosted runner thay vì ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v3
        
      - name: Deploy n8n
        run: |
          echo "Deploying n8n..."
          docker-compose down
          docker-compose pull
          docker-compose up -d
          
      - name: Send notification
        if: success() && (github.event_name == 'push' || github.event_name == 'workflow_dispatch')
        run: |
          if [[ ! -z "${{ secrets.TELEGRAM_BOT_TOKEN }}" && ! -z "${{ secrets.TELEGRAM_CHAT_ID }}" ]]; then
            curl -s -X POST https://api.telegram.org/bot${{ secrets.TELEGRAM_BOT_TOKEN }}/sendMessage \
              -d chat_id=${{ secrets.TELEGRAM_CHAT_ID }} \
              -d text="🚀 Deployed n8n successfully!" \
              -d parse_mode=Markdown
          fi
```

## Lưu ý quan trọng về bảo mật

- Runner sẽ có quyền truy cập vào server của bạn, chỉ sử dụng cho các repository đáng tin cậy
- Đảm bảo runner chỉ được cấu hình để chạy trên branch cụ thể (như `develop`)
- Cân nhắc sử dụng runner trong môi trường cách ly (Docker container) để tăng cường bảo mật