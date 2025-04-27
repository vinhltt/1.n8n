# Hướng dẫn: Tự động Backup n8n Workflows & Credentials lên GitHub

Workflow này sẽ tự động chạy hàng ngày, export dữ liệu n8n của bạn, và đẩy lên một kho chứa Git trên GitHub bằng SSH. Dựa trên file workflow `_Daily__Schedule_backup.json`.

## I. Chuẩn bị Môi trường (Thực hiện bên ngoài n8n)

Bước này rất quan trọng và cần thực hiện đúng trên server hoặc bên trong container Docker nơi n8n đang chạy.

1.  **Cài đặt Git:** Đảm bảo `git` đã được cài đặt. Nếu dùng Docker, bạn có thể cần cài thêm vào container hoặc sử dụng image có sẵn Git.
2.  **Tạo Repository trên GitHub:**
    * Truy cập GitHub và tạo một repository mới (có thể là private) để lưu trữ backup, ví dụ `n8n-backup-data`.
    * Ghi lại URL SSH của repository này (ví dụ: `git@github.com:YourUsername/n8n-backup-workflows.git`).
3.  **Tạo và Cấu hình SSH Keys:**
    * **Tạo Key:** Nếu chưa có SSH key cho user `node` (hoặc user chạy n8n), hãy tạo một cặp key mới. Chạy lệnh sau trong môi trường n8n (terminal hoặc Execute Command node một lần):
        ```bash
        # Tạo thư mục .ssh nếu chưa có và đặt quyền phù hợp
        mkdir -p ~/.ssh && chmod 700 ~/.ssh
        # Tạo key không cần passphrase, lưu vào file mặc định
        ssh-keygen -t rsa -b 4096 -f ~/.ssh/id_rsa -N ""
        ```
    * **Thêm Public Key vào GitHub:** Lấy nội dung file public key (thường là `~/.ssh/id_rsa.pub`) và thêm nó vào:
        * **Deploy Keys** của repository backup trên GitHub (nên chọn "Allow write access").
        * Hoặc vào **SSH and GPG keys** trong cài đặt tài khoản GitHub của bạn.
4.  **Xác định Thư mục Làm việc:** Workflow này giả định nó sẽ clone repo vào thư mục `repo` bên trong thư mục home (`~`) của user `node`.

## II. Thiết lập Workflow n8n

1.  **Import Workflow:** Tải file `_Daily__Schedule_backup.json` lên n8n của bạn thông qua chức năng Import.
2.  **Kiểm tra và Hiểu các Node:**

    * **`Cron` Node:** Kích hoạt workflow chạy hàng ngày vào lúc 7:15 sáng theo cấu hình mặc định. Bạn có thể thay đổi lịch trình này.
    * **`Export Workflows clone repo` (Node Setup & Clone/Pull):**
        * **Mục đích:** Node này thực hiện các bước thiết lập quan trọng và clone/pull repo.
        * **Lệnh:**
            ```bash
            # Tạo thư mục .ssh nếu chưa có và đặt quyền phù hợp
            mkdir -p ~/.ssh && \
            chmod 700 ~/.ssh && \
            # Lấy host key của github.com và thêm vào known_hosts (Giải quyết lỗi Host key verification failed)
            ssh-keyscan github.com >> ~/.ssh/known_hosts && \
            # XÓA THƯ MỤC 'repo' CŨ TRƯỚC KHI CLONE (Lưu ý: Luôn bắt đầu sạch)
            rm -rf ~/repo && \
            # Di chuyển về home và clone repo bằng SSH URL
            cd ~ && \
            git clone <link-repository> repo
            ```
        * **Cần điều chỉnh:** Thay `<link-repository>` bằng **URL SSH của repository bạn đã tạo ở Bước I.2**.
        * **Lưu ý:** Lệnh `rm -rf ~/repo` đảm bảo mỗi lần chạy workflow sẽ clone lại từ đầu. Nếu bạn muốn cập nhật repo hiện có thay vì clone lại, hãy thay `rm -rf ~/repo && cd ~ && git clone ... repo` bằng `(cd ~/repo && git pull) || (cd ~ && git clone ... repo)` (logic này sẽ pull nếu repo tồn tại, nếu không thì clone).
    * **`Export Workflows add config user git` (Node Cấu hình Git User):**
        * **Mục đích:** Cấu hình tên và email của người tạo commit để giải quyết lỗi `Author identity unknown`. Lệnh này nên được đặt để chạy *sau khi* repo đã được clone/pull thành công. *(Trong file JSON bạn cung cấp, node này đang đứng rời rạc, bạn cần kết nối nó vào luồng chính, ví dụ sau node Setup & Clone/Pull)*.
        * **Lệnh:**
            ```bash
            git config --global user.email <example@email.com> && git config --global user.name "N8N Backup Bot"
            ```
        * **Cần điều chỉnh:** Thay `"<example@email.com>"` và `"N8N Backup Bot"` bằng thông tin của bạn. Dùng `--global` sẽ áp dụng cho mọi repo của user `node`, hoặc bỏ `--global` và thêm `-C ~/repo` vào trước `config` để chỉ áp dụng cho repo này (`git -C ~/repo config user.email ...`).
    * **`Export Workflows` Node:**
        * **Mục đích:** Export tất cả workflows.
        * **Lệnh:** `npx n8n export:workflow --backup --output repo/{{ $now.toFormat("yyyyMMdd") }}/workflows/`
        * **Lưu ý:** Lệnh này đang lưu backup vào một thư mục con có tên theo ngày tháng (ví dụ `repo/20250427/workflows/`). Điều này tốt cho việc lưu trữ theo ngày, nhưng có thể làm repo Git phình to nhanh chóng nếu không có cơ chế dọn dẹp cũ.
    * **`Export Credentials` Node:**
        * **Mục đích:** Export credentials đã mã hóa.
        * **Lệnh:** `npx n8n export:credentials --backup --output repo/{{ $now.toFormat("yyyyMMdd") }}/credentials/`
        * **Lưu ý:** Tương tự node trên, lưu vào thư mục con theo ngày. Nhớ backup cả file encryption key của n8n một cách an toàn ở nơi khác!
    * **`git add` Node:**
        * **Mục đích:** Thêm tất cả các thay đổi (file export mới) vào staging area của Git.
        * **Lệnh:** `git -C repo add .` (Chạy lệnh `add .` bên trong thư mục `repo`).
    * **`git commit` Node:**
        * **Mục đích:** Tạo một commit mới với thông điệp chứa timestamp.
        * **Lệnh:** `git -C repo commit -m "Auto backup ({{ new Date().toISOString() }})"`.
    * **`git push` Node:**
        * **Mục đích:** Đẩy commit mới lên repository trên GitHub.
        * **Lệnh:** `git -C repo push`.

3.  **Kết nối các Node:**
    * Đảm bảo luồng chạy hợp lý: `Cron` -> `Setup & Clone/Pull` -> `Config Git User` -> `Export Workflows` -> `Export Credentials` -> `git add` -> `git commit` -> `git push`.
    * Xóa node `On clicking 'execute'` nếu bạn chỉ muốn chạy tự động theo `Cron`.

## III. Kích hoạt và Giám sát

1.  **Lưu và Kích hoạt:** Lưu lại workflow và bật (activate) nó lên.
2.  **Chạy thử:** Bạn có thể chạy thử workflow bằng cách nhấn nút "Execute Workflow" (nếu giữ lại node Manual Trigger) hoặc chờ đến giờ Cron để kiểm tra.
3.  **Kiểm tra GitHub:** Sau khi workflow chạy thành công, truy cập repository backup trên GitHub để xem các file workflows/credentials đã được đẩy lên hay chưa.
4.  **Giám sát:** Theo dõi lịch sử thực thi (Executions) trong n8n để phát hiện lỗi và khắc phục kịp thời.

Chúc mừng bạn đã thiết lập thành công quy trình backup tự động cho n8n!