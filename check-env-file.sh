#!/bin/bash
# check-env-file.sh - Kiểm tra nội dung file .env

# Hiển thị thông tin về việc file .env có tồn tại không
if [ -f .env ]; then
    echo "✅ File .env tồn tại trong thư mục hiện tại"
    
    # Kiểm tra nội dung của N8N_ENCRYPTION_KEY
    if grep -q "N8N_ENCRYPTION_KEY" .env; then
        echo "✅ Tìm thấy biến N8N_ENCRYPTION_KEY trong file .env"
        
        # Lấy giá trị của N8N_ENCRYPTION_KEY và kiểm tra chú thích
        ENCRYPTION_KEY_LINE=$(grep "N8N_ENCRYPTION_KEY" .env)
        echo "🔍 Dòng chứa ENCRYPTION_KEY: $ENCRYPTION_KEY_LINE"
        
        if echo "$ENCRYPTION_KEY_LINE" | grep -q "#"; then
            echo "⚠️ CẢNH BÁO: Tìm thấy chú thích (#) trong dòng ENCRYPTION_KEY"
            echo "💡 Đây có thể là nguyên nhân gây ra vấn đề với file config của n8n"
        else
            echo "✅ Không tìm thấy chú thích trong dòng ENCRYPTION_KEY"
        fi
    else
        echo "❌ Không tìm thấy biến N8N_ENCRYPTION_KEY trong file .env"
    fi
    
    # Hiển thị toàn bộ nội dung của file .env (che giấu giá trị thực)
    echo "📋 Nội dung file .env (đã che giấu giá trị):"
    cat .env | sed 's/\(N8N_[A-Z_]*=\)[^ #]*/\1[HIDDEN]/g'
else
    echo "❌ Không tìm thấy file .env trong thư mục hiện tại"
fi