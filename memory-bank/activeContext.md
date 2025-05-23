# activeContext.md

## Trọng tâm công việc hiện tại
- Hoàn thiện tài liệu BA Design chi tiết cho hệ thống PFM.
- Đảm bảo Memory Bank phản ánh đầy đủ nghiệp vụ, kiến trúc, yêu cầu chức năng/phi chức năng, tiến độ, insight mới nhất.
- **Bổ sung tính năng Quản lý Giao dịch Định kỳ (Recurring Transactions) vào Planning & Investment bounded context, nhằm hỗ trợ người dùng dự báo dòng tiền và chuẩn bị tiền cho từng tháng trong năm.**

## Thay đổi gần đây
- Cập nhật lại toàn bộ Memory Bank dựa trên BA Design.
- Làm rõ phạm vi, mục tiêu, nghiệp vụ, kiến trúc, pattern, công nghệ, ràng buộc kỹ thuật.
- Tổ chức lại các file unit test AccountServiceTests.cs bằng cách sử dụng **partial class AccountServiceTests** trải rộng trên nhiều file con theo từng method của service (ví dụ: AccountServiceTests.GetPagingAsync.cs, AccountServiceTests.CreateAsync.cs).
- **Đã di chuyển tất cả các file test liên quan đến AccountServiceTests vào thư mục con `AccountServiceTests` trong thư mục tests (`src/BE/CoreFinance/CoreFinance.Application.Tests/AccountServiceTests/`) để tổ chức theo service name.**
- Di chuyển hàm helper `GenerateFakeAccounts` vào TestHelpers.cs.
- **Đã quyết định bổ sung tính năng Quản lý Giao dịch Định kỳ vào bounded context Planning & Investment, sử dụng db_planning để lưu trữ mẫu giao dịch định kỳ và giao dịch dự kiến.**

## Bước tiếp theo
- Bổ sung chi tiết user stories, acceptance criteria cho từng chức năng.
- Làm việc với đội UI/UX để hoàn thiện mockup, wireframe.
- Xác định giải pháp frontend cụ thể.
- Chuẩn hóa checklist cho từng phase triển khai.
- **Chi tiết hóa thiết kế kỹ thuật và triển khai RecurringTransactionService trong Planning & Investment, bao gồm model RecurringTransactionTemplate và ExpectedTransaction.**

## Quyết định quan trọng
- Ưu tiên triển khai core service (Identity, Core Finance, Reporting) trước.
- Import statement: bắt đầu với manual upload, lên kế hoạch tích hợp API ngân hàng/aggregator sau.
- Đảm bảo mọi service có health check, logging, monitoring, alerting tự động.
- Định nghĩa rõ acceptance criteria, test coverage >80%.
- **Tính năng Quản lý Giao dịch Định kỳ (Recurring Transactions) được đặt trong bounded context Planning & Investment và sử dụng db_planning, thay vì Core Finance, để phù hợp với mục tiêu dự báo và lập kế hoạch tài chính.**

## Pattern, insight, lưu ý
- Mọi thay đổi lớn về nghiệp vụ, kiến trúc, công nghệ đều phải cập nhật vào Memory Bank.
- Đảm bảo các yêu cầu phi chức năng (NFR) được kiểm thử và giám sát liên tục.
- Luôn review lại BA Design và Memory Bank trước khi bắt đầu task mới.
- Đã chuẩn hóa sử dụng Bogus cho fake data trong unit test, tuân thủ .NET rule.
- Checklist phát triển/test phải có hướng dẫn sử dụng Bogus cho data giả lập.
- **Chỉ sử dụng xUnit cho unit test, không dùng NUnit hay framework khác.**
- **Chuẩn hóa sử dụng FluentAssertions cho assert kết quả trong unit test, tuân thủ .NET rule.**
- **Quy ước sử dụng instance AutoMapper thật (không mock) cho unit test ở tầng service, dựa vào các AutoMapper profile đã được cấu hình đúng và đã được test riêng.**
- **Lưu ý đặc biệt về việc đồng bộ dữ liệu giữa giao dịch dự kiến (ExpectedTransaction trong Planning & Investment) và giao dịch thực tế (Transaction trong Core Finance) thông qua Message Bus hoặc ReportingService.** 