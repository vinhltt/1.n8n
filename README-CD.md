# Hướng dẫn thiết lập Tự động Triển khai (CD) cho n8n qua Cloudflare Tunnel

File này hướng dẫn cách thiết lập GitHub Actions để tự động triển khai n8n khi có thay đổi trong các file docker-compose hoặc khi phát hiện phiên bản mới của n8n, sử dụng Cloudflare Tunnel thay vì mở port SSH.

## Ưu điểm khi sử dụng Cloudflare Tunnel

- **Không cần mở port**: Server không cần mở bất kỳ port nào ra bên ngoài, kể cả port SSH
- **Bảo mật cao**: Tất cả kết nối đều được khởi tạo từ server đến Cloudflare, không có kết nối đi vào
- **Mã hóa đầu cuối**: Tất cả traffic đều được mã hóa
- **Kiểm soát truy cập**: Có thể thêm Cloudflare Access để xác thực người dùng

## Cách hoạt động

GitHub Action workflow (`n8n-auto-deploy.yml`) sẽ tự động được kích hoạt trong các trường hợp:

1. Khi có commit mới vào các file `docker-compose.yml`, `docker-compose.override.yml` hoặc trong thư mục `cloudflared-config`
2. Hàng ngày (vào 2 giờ sáng) để kiểm tra phiên bản mới của n8n
3. Khi được kích hoạt thủ công qua GitHub interface

## Thiết lập Cloudflare Tunnel

1. **Tạo Cloudflare Tunnel**:
  - Đăng nhập vào Cloudflare Zero Trust dashboard: https://one.dash.cloudflare.com/
  - Chọn Access > Tunnels > Create a tunnel
  - Đặt tên cho tunnel (ví dụ: "n8n-tunnel")
  - Lưu lại token được cung cấp để sử dụng như biến môi trường `CLOUDFLARE_TUNNEL_TOKEN`

2. **Cấu hình DNS cho các endpoint**:
  - Tạo một bản ghi CNAME cho n8n của bạn:
    - Tên: n8n (hoặc tên miền phụ bạn muốn)
    - Giá trị: ID của tunnel (ví dụ: 12345678-abcd-efgh-ijkl-1234567890ab.cfargotunnel.com) 
  - Tạo một bản ghi CNAME cho SSH:
    - Tên: ssh (hoặc tên miền phụ bạn muốn)
    - Giá trị: ID của tunnel tương tự

3. **Cập nhật cấu hình Cloudflare**:
    - Điều chỉnh file `cloudflared-config/config.yml` để phản ánh tên miền thực tế của bạn

## Thiết lập Secrets trong GitHub

Để workflow hoạt động, bạn cần thiết lập các secrets sau trong repository GitHub:

1. Vào repository GitHub → Settings → Secrets and variables → Actions → New repository secret

2. Thêm các secrets sau:
  - `SSH_PRIVATE_KEY`: Khóa SSH private để xác thực với server
    ```
    # Nội dung của file ~/.ssh/id_rsa
    ```

  - `SSH_USER`: Tên người dùng SSH
    ```
    ubuntu, root, hoặc tên người dùng của bạn
    ```

  - `DEPLOY_PATH`: Đường dẫn đến thư mục chứa docker-compose files trên server
    ```
    /home/user/n8n hoặc đường dẫn khác
    ```

  - `CLOUDFLARE_TUNNEL_TOKEN`: Token của Cloudflare Tunnel
    ```
    eyJhbGciOiJSUzI1NiIsImtpZCI6IkNsb3VkZmxhcmUgVH... (token từ bước tạo tunnel)
    ```

  - (Tùy chọn) `TELEGRAM_BOT_TOKEN`: Token bot Telegram nếu muốn nhận thông báo
  - (Tùy chọn) `TELEGRAM_CHAT_ID`: ID chat Telegram để nhận thông báo

## Chuẩn bị Server

### 1. Cài đặt Docker và Docker Compose

Đảm bảo server của bạn đã cài đặt Docker và Docker Compose:

```bash
# Cài đặt Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Cài đặt Docker Compose
sudo apt-get update
sudo apt-get install -y docker-compose-plugin
```

### 2. Cài đặt và Cấu hình SSH Server

Nếu SSH server chưa được cài đặt (đặc biệt là khi gặp lỗi "Connection refused"), hãy cài đặt và kích hoạt nó:

```bash
# Cài đặt SSH server
sudo apt update
sudo apt install -y openssh-server

# Khởi động dịch vụ SSH
sudo systemctl enable ssh
sudo systemctl start ssh

# Kiểm tra trạng thái
sudo systemctl status ssh

# Kiểm tra kết nối đến localhost
ssh localhost
```

Nếu gặp vấn đề khi kết nối, hãy kiểm tra cấu hình SSH:

```bash
# Kiểm tra cấu hình SSH
cat /etc/ssh/sshd_config | grep "Port"

# Đảm bảo UFW cho phép SSH (nếu UFW được kích hoạt)
sudo ufw allow ssh
sudo ufw status
```

### 3. Thiết lập SSH key

Đảm bảo server của bạn có SSH key được cấu hình:

```bash
# Tạo thư mục .ssh nếu chưa tồn tại
mkdir -p ~/.ssh
chmod 700 ~/.ssh

# Thêm public key vào authorized_keys
echo "ssh-rsa AAAA..." >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
```

### 4. Chuẩn bị thư mục triển khai

Tạo thư mục để chứa các file docker-compose trên server:

```bash
mkdir -p /đường/dẫn/đến/thư/mục/triển/khai
```

## Kiểm tra thủ công

Bạn có thể kích hoạt workflow thủ công bằng cách:
1. Vào tab "Actions" trong repository GitHub
2. Chọn workflow "Auto Deploy n8n" 
3. Click "Run workflow"

## Lưu ý bảo mật

- **KHÔNG BAO GIỜ** commit file `.env` hoặc các file chứa mật khẩu lên repository
- Sử dụng Cloudflare Tunnel giúp tăng bảo mật vì không cần mở port nào trên server
- Đảm bảo file cấu hình `config.yml` không chứa thông tin nhạy cảm