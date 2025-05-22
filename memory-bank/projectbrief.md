# projectbrief.md

## Mục tiêu dự án
- Xây dựng hệ thống Quản lý Tài chính Cá nhân (PFM) theo kiến trúc microservices, sử dụng n8n làm workflow engine.
- Tự động hóa các quy trình tài chính cá nhân, giảm thiểu thao tác thủ công, tăng hiệu suất và độ chính xác.
- Đảm bảo khả năng mở rộng, bảo trì, tích hợp linh hoạt với các dịch vụ bên ngoài (ngân hàng, ví điện tử, v.v.).
- Cung cấp trải nghiệm người dùng trực quan, dễ sử dụng cho cả người không chuyên lập trình.
- Hỗ trợ người dùng dự báo dòng tiền thông qua quản lý giao dịch định kỳ, giúp chuẩn bị tài chính chủ động cho từng tháng trong năm.

## Phạm vi
- Bao gồm các bounded context: Identity & Access, Core Finance, Money Management, Planning & Investment, Reporting & Integration.
- Tích hợp nhiều dịch vụ tài chính, hỗ trợ import sao kê, quản lý tài khoản, giao dịch, ngân sách, mục tiêu, nợ, đầu tư.
- Cung cấp báo cáo tài chính cơ bản và nâng cao.
- Đảm bảo bảo mật, kiểm soát truy cập, tuân thủ các tiêu chuẩn bảo mật hiện hành.
- Quản lý giao dịch định kỳ (Recurring Transactions) như các khoản subscription (YouTube, Google One), hóa đơn (điện, nước), phí định kỳ khác, cho phép tạo dự báo và báo cáo kế hoạch tiền mặt theo tháng.

## Yêu cầu cốt lõi
- Dễ sử dụng, giao diện trực quan, thao tác nhanh chóng.
- Có tài liệu hướng dẫn chi tiết cho người dùng và nhà phát triển.
- Hỗ trợ backup, restore, triển khai linh hoạt trên nhiều môi trường.
- Đáp ứng các yêu cầu phi chức năng: hiệu năng, mở rộng, sẵn sàng cao, bảo mật, dễ bảo trì, tuân thủ quy định pháp lý nếu có.
- Chuẩn hóa sử dụng FluentAssertions cho assert kết quả trong unit test, tuân thủ .NET rule.
- Có khả năng dự báo dòng tiền chính xác, kết hợp giữa giao dịch thực tế và giao dịch dự kiến từ các mẫu định kỳ. 