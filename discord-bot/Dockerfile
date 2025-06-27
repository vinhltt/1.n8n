# Sử dụng Node.js 18 LTS Alpine image
FROM node:18-alpine

# Thiết lập thư mục làm việc
WORKDIR /app

# Cài đặt các dependencies hệ thống cần thiết
RUN apk add --no-cache \
    dumb-init \
    curl \
    && rm -rf /var/cache/apk/*

# Copy package.json và package-lock.json (nếu có)
COPY package*.json ./

# Cài đặt dependencies (sử dụng --omit=dev thay vì --only=production)
RUN npm ci --omit=dev && npm cache clean --force

# Copy source code và healthcheck script
COPY . .
COPY healthcheck.sh /usr/local/bin/healthcheck.sh
RUN chmod +x /usr/local/bin/healthcheck.sh

# Tạo user non-root để chạy ứng dụng
RUN addgroup -g 1001 -S nodejs && \
    adduser -S discordbot -u 1001

# Chuyển ownership của thư mục app cho user nodejs
RUN chown -R discordbot:nodejs /app
USER discordbot

# Expose port (không cần thiết cho Discord bot nhưng để sẵn cho health check)
EXPOSE 3000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD /usr/local/bin/healthcheck.sh || exit 1

# Sử dụng dumb-init để xử lý signals properly
ENTRYPOINT ["dumb-init", "--"]

# Chạy ứng dụng
CMD ["node", "index.js"] 