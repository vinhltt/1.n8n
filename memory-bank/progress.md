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
- **Đã hoàn thành triển khai Identity & Access Management System (Production-ready):**
  - **Triển khai đầy đủ Domain Layer: User, Role, ApiKey entities với audit fields và validation.**
  - **Triển khai Application Services: UserService, RoleService, ApiKeyService, AuthenticationService với async/await patterns.**
  - **Triển khai Infrastructure Layer: Repository pattern, Entity Framework Core, PostgreSQL integration.**
  - **Triển khai API Controllers: AuthController, UsersController, RolesController, ApiKeysController với RESTful design.**
  - **Triển khai SSO Integration: OpenIddict OAuth2/OIDC provider với authorization và token endpoints.**
  - **Triển khai Frontend Authentication: Vue.js/Nuxt login/signup UI với complete authentication flows.**
  - **Database Integration: PostgreSQL với EF Core migrations và seeded data.**
  - **Production Testing: Unit tests (1/1), Integration tests (4/4), Build validation, Security compliance.**
- **Đã hoàn thành triển khai đầy đủ tính năng Quản lý Giao dịch Định kỳ (Recurring Transactions) trong Core Finance:**
  - **Tạo domain models: RecurringTransactionTemplate và ExpectedTransaction với đầy đủ properties, enums và navigation properties.**
  - **Triển khai RecurringTransactionTemplateService với 9 methods chính: CRUD operations, quản lý trạng thái active, sinh giao dịch dự kiến, tính toán ngày thực hiện.**
  - **Triển khai ExpectedTransactionService với 11 methods chính: CRUD operations, quản lý lifecycle (confirm/cancel/adjust), dự báo dòng tiền.**
  - **Tạo đầy đủ DTOs cho cả hai services: CreateRequest, UpdateRequest, ViewModel và các specialized requests.**
  - **Viết comprehensive unit tests cho tất cả methods với coverage cao, áp dụng pattern partial class và tổ chức theo thư mục service.**
- **Đã cấu hình Dependency Injection cho Unit of Work, RecurringTransactionTemplateService và ExpectedTransactionService.**
- **Đã tạo các validator bằng FluentValidation cho các request DTOs liên quan đến RecurringTransactionTemplate và ExpectedTransaction, và đăng ký chúng tập trung.**
- **Đã triển khai Background Job Service sử dụng IHostedService để tự động sinh giao dịch dự kiến từ các mẫu định kỳ active hàng ngày.**
- **Đã triển khai đầy đủ Frontend Recurring Transactions Management với trang quản lý, modal CRUD, composable, types và menu navigation.**
- **Đã hoàn thành ExcelApi reorganization:**
  - **Di chuyển từ src/ExcelApi/ vào src/BE/ExcelApi/ để thống nhất cấu trúc backend.**
  - **Cập nhật solution structure, Docker configuration, và GitHub Actions integration.**
  - **ExcelApi fully functional với Excel processing capabilities và Swagger documentation.**
- **Đã bắt đầu triển khai Money Management bounded context:**
  - **Tạo solution MoneyManagement với cấu trúc Clean Architecture đầy đủ (6 projects).**
  - **Triển khai Domain Layer với Budget, Jar, SharedExpense, SharedExpenseParticipant entities.**
  - **Triển khai Base Infrastructure: BaseEntity, IBaseRepository, IUnitOfWork.**
  - **Hoàn thành BudgetService với đầy đủ business logic, DTOs, validators và AutoMapper profiles.**
  - **✅ HOÀN THÀNH Infrastructure Layer: BaseRepository implementation đầy đủ interface, UnitOfWork implementation, MoneyManagementDbContext với entity configurations.**
  - **✅ HOÀN THÀNH Contract Extensions: ModelBuilderExtensions, SqlParameterAttribute, CommonConst, StringHelper.**
  - **✅ Build thành công toàn bộ MoneyManagement solution (6/6 projects compile successfully).**

## Còn lại
- **Ưu tiên cao - Hoàn thiện Money Management bounded context:**
  - **Triển khai JarService với business logic cho hệ thống 6 Jars method.**
  - **Triển khai SharedExpenseService với logic quản lý chi tiêu nhóm và participants.**
  - **Tạo API Controllers cho Budget, Jar, SharedExpense.**
  - **Viết unit tests cho tất cả services trong Money Management.**
  - **Triển khai Frontend cho Money Management: Budget management, Jar management, Shared expense tracking.**
- **Triển khai PlanningInvestment bounded context:**
  - **Tạo Goal và Investment entities với validation và documentation.**
  - **Triển khai Infrastructure Layer: BaseRepository, UnitOfWork, DbContext cho PlanningInvestment.**
  - **Triển khai DebtService, GoalService, InvestmentService với đầy đủ business logic.**
  - **Tạo DTOs và interfaces cho tất cả entities trong PlanningInvestment.**
  - **Tạo API Controllers cho Debt, Goal, Investment.**
  - **Viết unit tests cho tất cả services trong PlanningInvestment.**
  - **Cấu hình dependency injection và Program.cs cho PlanningInvestment API.**
  - **Triển khai Frontend cho Planning & Investment features.**
- **Tích hợp và tối ưu hóa:**
  - **Tích hợp với NotificationService để gửi thông báo về giao dịch định kỳ sắp đến hạn.**
  - **Cập nhật ReportingService để tạo báo cáo kế hoạch tiền mặt kết hợp giao dịch thực tế và dự kiến.**
  - **Triển khai trang quản lý Expected Transactions để xem, xác nhận, hủy, điều chỉnh giao dịch dự kiến.**
  - **Tích hợp Expected Transactions vào Transaction page để hiển thị giao dịch dự kiến cùng với giao dịch thực tế.**
- Bổ sung user stories, acceptance criteria chi tiết cho từng chức năng còn lại.
- Hoàn thiện mockup, wireframe UI/UX.
- Chuẩn hóa checklist cho từng phase triển khai.
- Kiểm thử các yêu cầu phi chức năng (NFR) và giám sát liên tục.

## Trạng thái hiện tại
- Dự án đã có nền tảng tài liệu nghiệp vụ, kiến trúc, kỹ thuật vững chắc.
- **Identity & Access Management đã hoàn thành đầy đủ và production-ready: User, Role, ApiKey management, SSO integration, Frontend authentication.**
- **Core Finance bounded context đã có đầy đủ chức năng cơ bản: Account, Transaction, RecurringTransactionTemplate, ExpectedTransaction.**
- **Tính năng Recurring Transactions đã hoàn thành đầy đủ: Backend services, API Controllers, Background Job, Frontend UI.**
- **Background Job Service đang chạy tự động hàng ngày để sinh giao dịch dự kiến từ các mẫu định kỳ active.**
- **Frontend có trang quản lý Recurring Transactions hoàn chỉnh với CRUD operations, filtering, và modal interface.**
- **ExcelApi đã được reorganize vào cấu trúc BE và hoàn toàn functional với Docker support.**
- **Money Management bounded context đã bắt đầu triển khai với BudgetService hoàn chỉnh, nhưng thiếu Infrastructure Layer implementation.**
- **PlanningInvestment bounded context có cấu trúc project hoàn chỉnh (6 projects) và Debt entity với validation, nhưng thiếu Goal/Investment entities, toàn bộ Application Layer (services, DTOs, interfaces), Infrastructure Layer (repositories, DbContext), và API Controllers.**
- Sẵn sàng chuyển sang giai đoạn hoàn thiện Money Management Infrastructure và triển khai các services còn lại.
- **Pattern tổ chức unit test đã được chuẩn hóa và áp dụng thành công, có thể replicate cho các services khác.**

## Vấn đề đã biết
- Chưa có user stories và acceptance criteria chi tiết cho từng chức năng.
- Chưa kiểm thử thực tế các NFR (hiệu năng, bảo mật, reliability, compliance).
- **MoneyManagement Infrastructure Layer chưa được implement: BaseRepository, UnitOfWork, DbContext implementation.**
- **PlanningInvestment bounded context chỉ có cấu trúc project và Debt entity với enums, chưa có Goal/Investment entities, services implementation, DTOs, Infrastructure Layer, và API Controllers.**
- **Cần thiết kế cơ chế notification cho giao dịch định kỳ sắp đến hạn.**
- **Cần tích hợp dữ liệu giao dịch dự kiến vào báo cáo tài chính.**
- **Frontend chưa có integration với Money Management và PlanningInvestment APIs.**

## Tiến hóa quyết định
- Ưu tiên triển khai core service trước, import statement manual upload, test coverage >80%.
- Mọi thay đổi lớn đều phải cập nhật vào Memory Bank và BA Design.
- **Quyết định triển khai tính năng Recurring Transactions trong Core Finance thay vì Planning & Investment để tận dụng cơ sở hạ tầng sẵn có.**
- **Áp dụng pattern partial class cho unit test organization đã chứng minh hiệu quả, sẽ tiếp tục sử dụng cho các services khác.**
- **Thiết kế ExpectedTransaction với lifecycle management đầy đủ (Pending/Confirmed/Cancelled/Completed) để hỗ trợ các use case phức tạp.**
- **Triển khai Money Management theo từng service một, bắt đầu với BudgetService để tạo foundation cho các services khác.**
- **Hoàn thành Identity & Access Management trước các bounded contexts khác để có authentication/authorization foundation.**
- **Ưu tiên hoàn thiện MoneyManagement Infrastructure Layer trước khi triển khai JarService và SharedExpenseService.**
- **Reorganize ExcelApi vào cấu trúc BE để thống nhất với các bounded contexts khác.**