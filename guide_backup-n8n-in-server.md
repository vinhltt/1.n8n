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
    ```bash title="Dừng container n8n"
    docker stop n8n_main
    # Hoặc nếu bạn đang quản lý bằng docker-compose trong thư mục chứa file compose:
    # docker-compose stop n8n
    ```
2.  **Đảm bảo Container PostgreSQL đang chạy:** Database cần phải hoạt động để thực hiện khôi phục. Kiểm tra bằng lệnh `docker ps`. Nếu nó không chạy, hãy khởi động:
    ```bash title="Khởi động container PostgreSQL"
    docker start postgresdb
    # Hoặc nếu dùng docker-compose:
    # docker-compose up -d postgresdb
    ```

### Bước 1: Khôi phục Database PostgreSQL

1.  **Sao chép file backup SQL vào Container PostgreSQL:**
    Thay thế `/path/to/your/backup/folder/your_db_backup.sql.gz` bằng đường dẫn thực tế đến file backup database của bạn.
    ```bash title="Sao chép file backup SQL vào container PostgreSQL"
    docker cp /path/to/your/backup/folder/your_db_backup.sql.gz postgresdb:/tmp/n8n_restore_db.sql.gz
    ```

2.  **Thực thi lệnh khôi phục bên trong Container PostgreSQL:**
    **Quan trọng:** Thay thế `YOUR_DB_USER` và `YOUR_DB_PASSWORD` bằng các giá trị thực tế lấy từ file `.env` của bạn.
    
    * **Nếu backup là Plain SQL format (không phải custom format):**
    ```bash title="Khôi phục database PostgreSQL - Plain SQL Format"
    # Lệnh này sẽ giải nén, xóa DB cũ (nếu có), tạo DB mới và nạp dữ liệu
    docker exec -i -e PGPASSWORD=YOUR_DB_PASSWORD postgresdb bash -c ' \
        echo "--- Decompressing backup file ---" && \
        gunzip /tmp/n8n_restore_db.sql.gz && \
        echo "--- Dropping existing database (n8n_database) if exists ---" && \
        dropdb -U YOUR_DB_USER --if-exists n8n_database && \
        echo "--- Creating new database (n8n_database) ---" && \
        createdb -U YOUR_DB_USER n8n_database && \
        echo "--- Restoring database from /tmp/n8n_restore_db.sql ---" && \
        psql -U YOUR_DB_USER -d n8n_database < /tmp/n8n_restore_db.sql && \
        echo "--- Cleaning up temporary file ---" && \
        rm /tmp/n8n_restore_db.sql && \
        echo "--- Database restore completed ---" \
    '
    ```
    
    * **Nếu backup là PostgreSQL Custom Format (thông báo "PostgreSQL custom-format dump"):**
    ```bash title="Khôi phục database PostgreSQL - Custom Format"
    # Lệnh này sẽ giải nén, xóa DB cũ (nếu có), tạo DB mới và nạp dữ liệu bằng pg_restore
    docker exec -i -e PGPASSWORD=YOUR_DB_PASSWORD postgresdb bash -c ' \
        echo "--- Decompressing backup file ---" && \
        gunzip /tmp/n8n_restore_db.sql.gz && \
        echo "--- Dropping existing database (n8n_database) if exists ---" && \
        dropdb -U YOUR_DB_USER --if-exists n8n_database && \
        echo "--- Creating new database (n8n_database) ---" && \
        createdb -U YOUR_DB_USER n8n_database && \
        echo "--- Restoring database using pg_restore (for custom format) ---" && \
        pg_restore -U YOUR_DB_USER -d n8n_database /tmp/n8n_restore_db.sql && \
        echo "--- Cleaning up temporary file ---" && \
        rm /tmp/n8n_restore_db.sql && \
        echo "--- Database restore completed ---" \
    '
    ```
    
    * **Làm thế nào để biết định dạng file backup?** Nếu bạn không chắc chắn, hãy thử khôi phục bằng `psql` trước. Nếu gặp lỗi "The input is a PostgreSQL custom-format dump", hãy sử dụng phương pháp `pg_restore`.
    
    * Lệnh `dropdb --if-exists` sẽ xóa database `n8n_database` hiện có trước khi tạo mới và khôi phục.
    * Theo dõi output để đảm bảo không có lỗi xảy ra.

### Bước 2: Khôi phục Dữ liệu n8n (Docker Volume `n8n_data`)

1.  **Chạy container tạm thời để xóa dữ liệu cũ và giải nén backup vào volume:**
    **CẢNH BÁO:** Lệnh này sẽ **XÓA SẠCH** dữ liệu hiện có trong volume `n8n_data` trước khi giải nén.
    * Thay thế `/path/to/your/backup/folder` bằng đường dẫn thực tế đến thư mục chứa file backup.
    * Thay thế `your_n8n_data_backup.tar.gz` bằng tên file backup dữ liệu n8n thực tế của bạn.

    ```bash title="Khôi phục dữ liệu n8n từ backup"
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

```bash title="Khởi động lại n8n"
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

## Phương pháp khôi phục tự động bằng Script

Để đơn giản hóa quá trình khôi phục, chúng tôi đã chuẩn bị các script tự động giúp bạn thực hiện các bước khôi phục một cách dễ dàng hơn. Các script này đặc biệt hữu ích khi bạn gặp phải các vấn đề phổ biến trong quá trình khôi phục thủ công.

### Khôi phục Database bằng Script `restore-n8n-db.sh`

Script này tự động thực hiện toàn bộ quá trình khôi phục database, bao gồm:
- Tự động phát hiện định dạng file backup (plain SQL hoặc custom format)
- Xử lý trường hợp file đã được giải nén từ trước
- Xử lý lỗi và ghi log chi tiết

**Cách sử dụng:**

1. **Chỉnh sửa thông tin trong script**:
   ```bash title="Mở và chỉnh sửa script"
   # Mở file bằng trình soạn thảo
   nano restore-n8n-db.sh
   
   # Chỉnh sửa các biến sau:
   BACKUP_FILE="$HOME/path/to/your/n8n_db_backup_xxx.sql.gz"  # Đường dẫn đến file backup
   PG_USER="your_db_user"                                      # Tên user database
   PG_PASSWORD="your_db_password"                              # Mật khẩu database
   ```

2. **Cấp quyền thực thi**:
   ```bash title="Cấp quyền thực thi"
   chmod +x restore-n8n-db.sh
   ```

3. **Chạy script**:
   ```bash title="Chạy script khôi phục database"
   ./restore-n8n-db.sh
   ```

### Khôi phục Dữ liệu n8n bằng Script `restore-n8n-data.sh`

Script này giải quyết nhiều vấn đề thường gặp khi khôi phục dữ liệu n8n, đặc biệt là trên macOS:
- Tránh lỗi "Mounts denied" trên macOS Docker Desktop
- Tự động xử lý quyền truy cập file
- Báo cáo chi tiết quá trình khôi phục

**Cách sử dụng:**

1. **Chỉnh sửa thông tin trong script**:
   ```bash title="Mở và chỉnh sửa script"
   nano restore-n8n-data.sh
   
   # Chỉnh sửa biến sau:
   BACKUP_FILE="$HOME/path/to/your/n8n_data_backup_xxx.tar.gz"  # Đường dẫn đến file backup
   DOCKER_VOLUME="n8n_data"                                     # Tên Docker volume
   ```

2. **Cấp quyền thực thi**:
   ```bash title="Cấp quyền thực thi"
   chmod +x restore-n8n-data.sh
   ```

3. **Chạy script**:
   ```bash title="Chạy script khôi phục dữ liệu"
   ./restore-n8n-data.sh
   ```

## Xử lý tình huống đặc biệt

### Lỗi truy cập đường dẫn trên macOS Docker Desktop

Nếu bạn gặp lỗi "Mounts denied" hoặc "The path is not shared from the host" khi chạy Docker trên macOS:

1. **Sử dụng script `restore-n8n-data.sh`** (khuyến nghị)
   - Script này sử dụng thư mục tạm thời ở `/tmp` để tránh các vấn đề về chia sẻ file trên macOS

2. **Hoặc cấu hình chia sẻ file trong Docker Desktop**:
   - Mở Docker Desktop > Preferences > Resources > File Sharing
   - Thêm thư mục chứa file backup của bạn
   - Khởi động lại Docker Desktop

### Lỗi "File already exists" khi giải nén

Nếu bạn gặp lỗi như "gzip: file already exists" khi khôi phục database:

1. **Sử dụng script `restore-n8n-db.sh`** (khuyến nghị)
   - Script này tự động kiểm tra và xóa các file tạm thời đã giải nén

2. **Hoặc thực hiện thủ công**:
   ```bash title="Xóa file tạm thời đã giải nén"
   docker exec -it postgresdb bash -c 'rm -f /tmp/n8n_restore_db.sql'
   ```
   Sau đó thực hiện lại quá trình khôi phục.

### Lỗi "The input is a PostgreSQL custom-format dump"

Nếu bạn gặp lỗi này khi sử dụng lệnh `psql`:

1. **Sử dụng script `restore-n8n-db.sh`** (khuyến nghị)
   - Script này tự động phát hiện và xử lý cả hai định dạng backup

2. **Hoặc sử dụng `pg_restore` thay vì `psql`**:
   ```bash title="Khôi phục PostgreSQL custom format dump"
   docker exec -i -e PGPASSWORD=YOUR_DB_PASSWORD postgresdb bash -c '
       pg_restore -U YOUR_DB_USER -d n8n_database /tmp/n8n_restore_db.sql
   '
   ```

## Phiên bản và Tương thích

Hướng dẫn này được cập nhật vào tháng 5/2025 và đã được kiểm tra với các phiên bản sau:
- Docker: 24.x+
- n8n: 1.x+
- PostgreSQL: 14+
- Hệ điều hành: Ubuntu 22.04+, macOS 14+

## Các lỗi thường gặp và cách khắc phục

| Lỗi | Nguyên nhân có thể | Cách khắc phục |
|-----|-----------------|--------------|
| Lỗi truy cập vào volume | Quyền truy cập không đủ | Chạy lệnh với `sudo` hoặc kiểm tra quyền của user Docker |
| Lỗi kết nối database | PostgreSQL không hoạt động | Đảm bảo container PostgreSQL đang chạy: `docker ps \| grep postgres` |
| n8n không hiển thị workflows sau khi khôi phục | Sai encryption key | Kiểm tra lại `N8N_ENCRYPTION_KEY` trong file `.env` |
| Credentials không hoạt động | File config bị lỗi | Khôi phục file config từ backup hoặc cập nhật encryption key |

## Những lưu ý bổ sung

- **Kiểm tra encryption key**: Đảm bảo rằng `N8N_ENCRYPTION_KEY` trong file `.env` của bạn giống với giá trị được sử dụng khi tạo backup. Nếu không, các credentials sẽ không hoạt động.
- **Sao lưu thường xuyên**: Lập lịch sao lưu tự động hàng ngày để luôn có các phiên bản backup gần nhất.
- **Kiểm tra tính toàn vẹn của backup**: Định kỳ thử nghiệm khôi phục trên môi trường staging để đảm bảo backup của bạn hoạt động đúng.
- **Lưu trữ an toàn**: Lưu trữ các file backup ở nhiều vị trí khác nhau (local, cloud storage) để tránh mất dữ liệu.

Nếu bạn gặp bất kỳ vấn đề nào không được đề cập trong hướng dẫn này, hãy tham khảo tài liệu chính thức của n8n hoặc tạo issue trên repository GitHub của dự án.