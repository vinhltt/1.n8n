# progress.md

## Đã hoàn thành
- Hoàn thiện tài liệu BA Design chi tiết cho hệ thống PFM.
- Cập nhật đầy đủ các file core trong Memory Bank theo BA Design.
- Xác định rõ phạm vi, mục tiêu, nghiệp vụ, kiến trúc, pattern, công nghệ, ràng buộc kỹ thuật.
- Đã chuẩn hóa sử dụng Bogus cho fake data trong unit test, tuân thủ .NET rule.
- Đã thống nhất chỉ sử dụng xUnit cho unit test.
- Đã chuẩn hóa sử dụng FluentAssertions cho assert kết quả trong unit test, tuân thủ .NET rule.
- Đã tổ chức lại file unit test AccountServiceTests.cs bằng cách sử dụng **partial class** trải rộng trên nhiều file con theo từng method của service (AccountServiceTests.MethodName.cs) và di chuyển helper sang TestHelpers.cs.
- **Đã di chuyển các file test của AccountServiceTests vào thư mục con `AccountServiceTests` để tổ chức test theo service name.**
- **Đã quyết định bổ sung tính năng Quản lý Giao dịch Định kỳ vào bounded context Planning & Investment để hỗ trợ người dùng dự báo dòng tiền và chuẩn bị tiền cho từng tháng trong năm.**

## Còn lại
- Bổ sung user stories, acceptance criteria chi tiết cho từng chức năng.
- Hoàn thiện mockup, wireframe UI/UX.
- Xác định giải pháp frontend cụ thể.
- Chuẩn hóa checklist cho từng phase triển khai.
- Kiểm thử các yêu cầu phi chức năng (NFR) và giám sát liên tục.
- **Thiết kế chi tiết và triển khai tính năng Quản lý Giao dịch Định kỳ (Recurring Transactions), bao gồm:**
  - **RecurringTransactionService trong Planning & Investment.**
  - **Model RecurringTransactionTemplate và ExpectedTransaction.**
  - **Cơ chế sinh giao dịch dự kiến (background worker hoặc event-driven).**
  - **Tích hợp với NotificationService để thông báo về giao dịch định kỳ sắp đến hạn.**
  - **Cập nhật ReportingService để tạo báo cáo kế hoạch tiền mặt kết hợp giao dịch thực tế và dự kiến.**
  - **Giao diện người dùng cho việc quản lý mẫu giao dịch định kỳ.**

## Trạng thái hiện tại
- Dự án đã có nền tảng tài liệu nghiệp vụ, kiến trúc, kỹ thuật vững chắc.
- Sẵn sàng chuyển sang giai đoạn chi tiết hóa chức năng, thiết kế UI/UX, chuẩn bị phát triển.
- **Đang trong quá trình chi tiết hóa yêu cầu nghiệp vụ và thiết kế kỹ thuật cho tính năng Quản lý Giao dịch Định kỳ trong Planning & Investment.**

## Vấn đề đã biết
- Chưa có user stories và acceptance criteria chi tiết cho từng chức năng.
- Chưa xác định giải pháp frontend cụ thể.
- Chưa kiểm thử thực tế các NFR (hiệu năng, bảo mật, reliability, compliance).
- **Cần giải quyết thách thức về đồng bộ dữ liệu giữa giao dịch dự kiến (ExpectedTransaction trong Planning & Investment) và giao dịch thực tế (Transaction trong Core Finance) để tạo báo cáo chính xác.**

## Tiến hóa quyết định
- Ưu tiên triển khai core service trước, import statement manual upload, test coverage >80%.
- Mọi thay đổi lớn đều phải cập nhật vào Memory Bank và BA Design.
- **Quyết định đặt tính năng Quản lý Giao dịch Định kỳ trong Planning & Investment thay vì Core Finance để phù hợp với mục tiêu dự báo và lập kế hoạch tài chính.** 