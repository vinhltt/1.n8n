#!/bin/bash
# cleanup-docker-n8n.sh - Các lệnh để xóa hoàn toàn n8n trong Docker

# Hiển thị hướng dẫn
echo "==== LỆNH XÓA DOCKER CONTAINERS, VOLUMES VÀ IMAGES CHO N8N ===="
echo "Chạy từng lệnh dưới đây để xóa các thành phần Docker liên quan đến n8n:"

echo -e "\n1. Dừng và xóa tất cả containers đang chạy:"
echo "   docker-compose down"

echo -e "\n2. Xóa file .env (có thể chứa thông tin khóa mã hóa không đúng):"
echo "   rm -f .env"

echo -e "\n3. Xóa thư mục n8n_data:"
echo "   rm -rf ./n8n_data"

echo -e "\n4. Liệt kê tất cả Docker volumes:"
echo "   docker volume ls"

echo -e "\n5. Xóa các Docker volumes cụ thể:"
echo "   docker volume rm n8n_data postgresdb_data"

echo -e "\n6. Hoặc xóa tất cả Docker volumes không sử dụng (cẩn thận):"
echo "   docker volume prune -f"

echo -e "\n7. Liệt kê tất cả Docker images:"
echo "   docker images"

echo -e "\n8. Xóa các Docker images cụ thể:"
echo "   docker rmi docker.n8n.io/n8nio/n8n postgres:15"

echo -e "\n9. Liệt kê tất cả Docker networks:"
echo "   docker network ls"

echo -e "\n10. Xóa network n8n:"
echo "    docker network rm n8n-network"

echo -e "\n11. Tạo lại thư mục n8n_data và ssh:"
echo "    mkdir -p ./n8n_data ./ssh"

echo -e "\n12. Kiểm tra không còn containers nào đang chạy:"
echo "    docker ps -a"

echo -e "\n==== LỆNH XÓA TẤT CẢ (THẬN TRỌNG) ===="
echo "# Dừng tất cả containers"
echo "docker-compose down"
echo ""
echo "# Xóa file .env"
echo "rm -f .env"
echo ""
echo "# Xóa thư mục n8n_data"
echo "rm -rf ./n8n_data"
echo ""
echo "# Xóa volumes"
echo "docker volume rm n8n_data postgresdb_data"
echo ""
echo "# Xóa images"
echo "docker rmi docker.n8n.io/n8nio/n8n postgres:15"
echo ""
echo "# Xóa network"
echo "docker network rm n8n-network"
echo ""
echo "# Tạo lại thư mục n8n_data và ssh"
echo "mkdir -p ./n8n_data ./ssh"
echo ""
echo "# Chú ý: Sau khi thực hiện các lệnh trên, hãy kích hoạt GitHub Action để triển khai lại n8n"