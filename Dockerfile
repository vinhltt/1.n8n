# Dockerfile để tùy chỉnh image n8n với thêm thư viện msoffcrypto-tool
# Sử dụng image n8n chính thức làm base image
FROM docker.n8n.io/n8nio/n8n

# Chuyển sang người dùng root để cài đặt gói
USER root

# Cài đặt Python và pip sử dụng apk (Alpine Linux package manager)
# và sau đó cài đặt msoffcrypto-tool
# Sử dụng --break-system-packages để giải quyết vấn đề với PEP 668
RUN apk add --no-cache python3 py3-pip && \
    pip3 install --no-cache-dir --break-system-packages msoffcrypto-tool && \
    # Đảm bảo các công cụ cơ bản được cài đặt
    apk add --no-cache bash curl && \
    # Xóa cache để giảm kích thước image
    rm -rf /var/cache/apk/*

# Trở lại người dùng node (người dùng mặc định trong container n8n)
USER node

# Thông tin về image
LABEL maintainer="VinhLTT <vinhltt.dev@gmail.com>"
LABEL description="n8n with msoffcrypto-tool for handling encrypted Office documents"