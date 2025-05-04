# Dockerfile để tùy chỉnh image n8n với thêm thư viện msoffcrypto-tool
# Sử dụng image n8n chính thức làm base image
FROM docker.n8n.io/n8nio/n8n

# Chuyển sang người dùng root để cài đặt gói
USER root

# Cài đặt các dependencies cần thiết và msoffcrypto-tool
RUN apt-get update && \
    apt-get install -y python3 python3-pip && \
    pip3 install msoffcrypto-tool && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Trở lại người dùng node (người dùng mặc định trong container n8n)
USER node

# Thông tin về image
LABEL maintainer="VinhLTT <vinhltt.dev@gmail.com>"
LABEL description="n8n with msoffcrypto-tool for handling encrypted Office documents"