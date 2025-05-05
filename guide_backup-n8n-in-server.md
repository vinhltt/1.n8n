# Hướng dẫn Khôi phục Hệ thống n8n từ Backup

Tài liệu này hướng dẫn các bước để khôi phục lại hệ thống n8n của bạn (bao gồm database PostgreSQL và dữ liệu cấu hình/workflows) từ các file backup đã được tạo bởi script `backup-n8n.sh`.

**CẢNH BÁO QUAN TRỌNG:**
* **GHI ĐÈ DỮ LIỆU:** Quá trình khôi phục sẽ **XÓA SẠCH** dữ liệu hiện tại của database và volume n8n, sau đó thay thế bằng dữ liệu từ file backup. Mọi thay đổi được thực hiện trên n8n kể từ thời điểm tạo bản backup sẽ bị **MẤT**.
* **THỬ NGHIỆM TRƯỚC:** **Luôn luôn** thử nghiệm quy trình khôi phục này trên một môi trường thử nghiệm (staging) trước khi áp dụng trên môi trường sản xuất (production) để đảm bảo các file backup hoạt động chính xác và bạn đã quen với quy trình.
* **SAO LƯU TRƯỚC KHI KHÔI PHỤC (TÙY CHỌN):** Nếu bạn không chắc chắn, hãy tạo một bản backup cuối cùng của trạng thái hiện tại trước khi bắt đầu khôi phục.

## Thông tin cần thiết (Prerequisites)

Trước khi bắt đầu, hãy đảm bảo bạn có các thông tin và file sau:

1.  **File Backup Database:** File `.sql.gz` chứa dữ liệu database PostgreSQL (ví dụ: `n8n_db_backup_20250427_083000.sql.gz`).
2.  **File Backup Dữ liệu n8n:** File `.tar.gz` chứa dữ liệu từ Docker Volume `n8n_data` (ví dụ: `n8n_data_backup_20250427_083000.tar.gz`).
3.  **Đường dẫn đến thư mục chứa backup:** Ví dụ: `/path/to/your/backup/folder`.
4.  **Thông tin Database (từ file `.env`):**
    * `POSTGRES_USER`: Tên người dùng database PostgreSQL.
    * `POSTGRES_PASSWORD`: Mật khẩu database PostgreSQL.
5.  **Tên Database:** `n8n_database` (theo cấu hình trong `docker-compose.override.yml`).
6.  **Tên Container PostgreSQL:** `postgresdb` (theo cấu hình trong `docker-compose.override.yml`).
7.  **Tên Container n8n:** `n8n_main` (theo cấu hình trong `docker-compose.override.yml`).
8.  **Tên Docker Volume n8n:** `n8n_data` (theo cấu hình trong `docker-compose.yml`).

## Các bước Khôi phục

### Bước 0: Chuẩn bị

1.  **Dừng Container n8n:** Để ngăn chặn việc ghi dữ liệu trong quá trình khôi phục.
    ```bash
    docker stop n8n_main
    # Hoặc nếu bạn đang quản lý bằng docker-compose trong thư mục chứa file compose:
    # docker-compose stop n8n
    ```
2.  **Đảm bảo Container PostgreSQL đang chạy:** Database cần phải hoạt động để thực hiện khôi phục. Kiểm tra bằng lệnh `docker ps`. Nếu nó không chạy, hãy khởi động:
    ```bash
    docker start postgresdb
    # Hoặc nếu dùng docker-compose:
    # docker-compose up -d postgresdb
    ```

### Bước 1: Khôi phục Database PostgreSQL

1.  **Sao chép file backup SQL vào Container PostgreSQL:**
    Thay thế `/path/to/your/backup/folder/your_db_backup.sql.gz` bằng đường dẫn thực tế đến file backup database của bạn.
    ```bash
    docker cp /path/to/your/backup/folder/your_db_backup.sql.gz postgresdb:/tmp/n8n_restore_db.sql.gz
    ```

2.  **Thực thi lệnh khôi phục bên trong Container PostgreSQL:**
    **Quan trọng:** Thay thế `YOUR_DB_USER` và `YOUR_DB_PASSWORD` bằng các giá trị thực tế lấy từ file `.env` của bạn.
    ```bash
    # Lệnh này sẽ giải nén, xóa DB cũ (nếu có), tạo DB mới và nạp dữ liệu
    docker exec -i -e PGPASSWORD=YOUR_DB_PASSWORD postgresdb bash -c ' \
        echo "--- Decompressing backup file ---" && \
        gunzip -f /tmp/n8n_restore_db.sql.gz && \
        echo "--- Dropping existing database (n8n_database) if exists ---" && \
        dropdb -U YOUR_DB_USER --if-exists n8n_database && \
        echo "--- Creating new database (n8n_database) ---" && \
        createdb -U YOUR_DB_USER n8n_database && \
        echo "--- Restoring database from /tmp/n8n_restore_db.sql ---" && \
        pg_restore -U YOUR_DB_USER -d n8n_database -v /tmp/n8n_restore_db.sql && \
        echo "--- Cleaning up temporary file ---" && \
        rm /tmp/n8n_restore_db.sql && \
        echo "--- Database restore completed ---" \
    '
    ```
    * Lệnh `dropdb --if-exists` sẽ xóa database `n8n_database` hiện có trước khi tạo mới và khôi phục.
    * `pg_restore` được sử dụng thay vì `psql` vì các file backup thường là định dạng custom PostgreSQL, không phải SQL thuần.
    * Theo dõi output để đảm bảo không có lỗi xảy ra.

    **Lưu ý quan trọng:** Nếu file backup của bạn là định dạng SQL thuần (không phải custom format), bạn sẽ cần sử dụng `psql` thay vì `pg_restore`. Bạn có thể xác định bằng cách kiểm tra thông báo lỗi khi khôi phục. Nếu gặp lỗi "The input is a PostgreSQL custom-format dump", hãy sử dụng `pg_restore` như trong ví dụ.

### Bước 2: Khôi phục Dữ liệu n8n (Docker Volume `n8n_data`)

1.  **Chạy container tạm thời để xóa dữ liệu cũ và giải nén backup vào volume:**
    **CẢNH BÁO:** Lệnh này sẽ **XÓA SẠCH** dữ liệu hiện có trong volume `n8n_data` trước khi giải nén.
    * Thay thế `/path/to/your/backup/folder` bằng đường dẫn thực tế đến thư mục chứa file backup.
    * Thay thế `your_n8n_data_backup.tar.gz` bằng tên file backup dữ liệu n8n thực tế của bạn.

    ```bash
    docker run --rm \
        -v n8n_data:/volume_data \
        -v /path/to/your/backup/folder:/backup_source \
        alpine \
        sh -c ' \
            echo "--- WARNING: Clearing existing data in volume n8n_data ---" && \
            rm -rf /volume_data/* && \
            echo "--- Restoring data from /backup_source/your_n8n_data_backup.tar.gz ---" && \
            tar xzf /backup_source/your_n8n_data_backup.tar.gz -C /volume_data && \
            echo "--- Volume n8n_data restore completed ---" \
        '
    ```
    * `-v n8n_data:/volume_data`: Mount volume `n8n_data` vào container tạm.
    * `-v /path/to/your/backup/folder:/backup_source`: Mount thư mục backup từ host vào container tạm.
    * `rm -rf /volume_data/*`: Xóa nội dung cũ trong volume.
    * `tar xzf ... -C /volume_data`: Giải nén file backup vào volume.
    * Theo dõi output để đảm bảo không có lỗi xảy ra.

### Bước 3: Khởi động lại n8n

Sau khi cả database và volume đã được khôi phục thành công:

```bash
docker start n8n_main
# Hoặc nếu dùng docker-compose:
# docker-compose up -d n8n
```
### Bước 4: Kiểm tra
* Mở trình duyệt và truy cập vào giao diện web n8n của bạn.
* Đăng nhập (nếu cần).
* Kiểm tra kỹ lưỡng:
    * Các Workflows có hiển thị đúng như trong bản backup không?
    * Các Credentials (thông tin đăng nhập dịch vụ) có còn đó không? (Lưu ý: Bạn có thể cần kích hoạt lại một số credentials).
    * Lịch sử thực thi (Executions) cũ có hiển thị không (nếu bạn không cấu hình tự động xóa chúng)?
* Nếu mọi thứ hoạt động như mong đợi, quá trình khôi phục đã thành công.

## Sử dụng Script Khôi phục Tự Động

Ngoài hướng dẫn thủ công ở trên, bạn cũng có thể sử dụng script `restore-n8n-db.sh` đã được cấu hình sẵn để khôi phục database:

```bash
# Di chuyển đến thư mục chứa script
cd /path/to/your/n8n/folder

# Cấp quyền thực thi nếu cần
chmod +x restore-n8n-db.sh

# Chạy script khôi phục database (sử dụng đường dẫn mặc định là /tmp/n8n_restore_db.sql.gz)
./restore-n8n-db.sh

# Hoặc chỉ định đường dẫn đến file backup
./restore-n8n-db.sh /path/to/your/backup/n8n_db_backup.sql.gz
```

Script này sẽ tự động xử lý các bước khôi phục database, bao gồm việc phát hiện định dạng file backup và sử dụng công cụ phù hợp (`pg_restore` hoặc `psql`) để khôi phục.