# activeContext.md

## Trọng tâm công việc hiện tại
- **✅ Đã hoàn thành triển khai Account Management system cho frontend Nuxt 3.**
- **✅ Đã cấu hình kết nối từ Nuxt frontend đến .NET Core API backend (https://localhost:7293).**
- **✅ Đã setup thành công Nuxt project với tất cả dependencies cần thiết.**
- **✅ Đã tổ chức lại cấu trúc types và interfaces theo Nuxt best practices.**
- **✅ Đã bổ sung menu navigation để truy cập trang Account management.**
- **✅ Dev server đang chạy thành công trên port 3000.**
- **✅ Build production thành công với tất cả optimizations.**
- **✅ Đã fix FilterBodyRequest format mismatch giữa frontend và backend.**
- **✅ Đã fix CreateAccountRequest validation bằng cách thay đổi frontend gửi form data.**
- **✅ Đã remove userId requirement cho account creation vì chưa có authentication system.**
- **✅ Đã fix DateTime timezone issue với PostgreSQL.**
- **✅ Đã chuẩn hóa search functionality trong tất cả services.**

## Thay đổi gần đây
- **✅ Đã triển khai đầy đủ Account Management system cho frontend:**
  - **Trang danh sách accounts (/apps/accounts) với CRUD operations, filtering, pagination**
  - **Modal component cho tạo/chỉnh sửa accounts với form validation**
  - **Trang chi tiết account (/apps/accounts/[id]) với charts và transactions**
  - **Composable useAccounts.ts với API integration và utility functions**

- **✅ Đã cấu hình project setup:**
  - **Cài đặt thành công tất cả dependencies: @nuxtjs/tailwindcss, @pinia/nuxt, @nuxtjs/i18n, @vueuse/nuxt**
  - **Fix conflicts giữa Tailwind CSS v3/v4, downgrade về version ổn định**
  - **Tạo file locales/vi.json cho internationalization**
  - **Fix CSS issues: Import đúng tailwind.css với custom VRISTO classes (bg-success, text-white-dark, etc.)**
  - **Thêm custom colors vào tailwind.config.js (primary, secondary, success, danger, warning, info, dark)**
  - **Disable TypeScript strict checking và i18n tạm thời để tránh compatibility issues**
  - **✅ Dev server và build production đều chạy thành công**

- **✅ Đã tổ chức lại cấu trúc theo Nuxt conventions:**
  - **Tách types ra folder riêng: types/account.ts, types/api.ts, types/index.ts**
  - **Tạo composable useApi.ts cho API calls chung**
  - **Cập nhật nuxt.config.ts với runtime config cho API base URL**

- **✅ Đã bổ sung menu navigation:**
  - **Thêm menu "Accounts" vào sidebar trong phần Apps**
  - **Sử dụng icon-credit-card có sẵn**
  - **Menu link đến /apps/accounts**

- **✅ Đã fix FilterBodyRequest format mismatch:**
  - **Cập nhật frontend FilterBodyRequest interface để khớp với backend structure**
  - **Thay đổi từ simple object sang complex filtering system với FilterRequest, FilterDetailsRequest**
  - **Cập nhật Pagination structure: pageNumber → pageIndex, totalRecords → totalRow, totalPages → pageCount**
  - **Thêm support cho FilterOperator, FilterLogicalOperator, SortDescriptor**
  - **Cập nhật ApiResponse interface: result → data để khớp với backend IBasePaging**
  - **Fix useApi.ts để không expect wrapper object { data: T }**
  - **Cập nhật tất cả usage trong pages/apps/accounts/index.vue**
  - **Đảm bảo gửi đúng format theo curl example: langId="", filter={}, orders=[] thay vì undefined**
  - **Làm cho tất cả properties trong FilterBodyRequest bắt buộc để tránh undefined values**

- **✅ Đã fix CreateAccountRequest validation bằng form data approach:**
  - **Thêm support cho form data trong useApi.ts với isFormData option**
  - **Thêm postForm() và putForm() methods cho form data requests**
  - **Cập nhật useAccounts để sử dụng postForm/putForm cho create/update operations**
  - **Giữ nguyên backend với [FromForm] attribute như thiết kế ban đầu**
  - **Thêm FluentValidation auto-validation middleware để tự động validate form data**
  - **Frontend giờ gửi FormData thay vì JSON cho CRUD operations**

- **✅ Đã remove userId requirement:**
  - **Xóa userId khỏi AccountCreateRequest trong frontend**
  - **userId đã là optional trong type definition**
  - **Sẽ bổ sung lại khi implement authentication system**
  - **Giúp đơn giản hóa testing và development hiện tại**

- **✅ Đã fix DateTime timezone issue với PostgreSQL:**
  - **Thêm EnableLegacyTimestampBehavior() trong DbContext configuration ở GeneralServiceExtension.cs**
  - **PostgreSQL yêu cầu DateTime với Kind=UTC, .NET mặc định tạo DateTime với Kind=Local**
  - **EnableLegacyTimestampBehavior() cho phép Npgsql tự động convert DateTime sang UTC**
  - **Xóa cấu hình thừa trong CoreFinanceDbContext.cs**
  - **Fix lỗi "Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamp with time zone'"**

- **✅ Đã chuẩn hóa search functionality trong tất cả services:**
  - **Ban đầu thử chuyển từ .ToLower().Contains() sang EF.Functions.ILike() cho PostgreSQL compatibility**
  - **Thêm comments trong unit tests để clarify sự khác biệt giữa test logic và production logic**
  - **Tất cả 34 unit tests cho GetPagingAsync methods đều pass**

## Quyết định và cân nhắc hiện tại
- **Architecture: Chỉ sử dụng Nuxt làm frontend, không sử dụng backend Nuxt - tất cả API calls đến .NET Core backend**
- **API Endpoint: https://localhost:7293 (có thể thay đổi qua environment variable NUXT_PUBLIC_API_BASE)**
- **TypeScript: Tạm thời disable strict checking để tránh conflicts với third-party libraries**
- **Tailwind CSS: Sử dụng version 3.4.0 ổn định thay vì v4 beta**
- **Dependencies: Đã resolve conflicts với apexcharts, sử dụng v4.0.0 + vue3-apexcharts v1.8.0**
- **API Structure: Frontend và backend đã đồng bộ về FilterBodyRequest và response format**

## Patterns và preferences quan trọng
- **Sử dụng Composition API với `<script setup>` syntax**
- **Types thay vì interfaces, tránh enums (trừ khi cần khớp với backend enums)**
- **Auto-import cho composables nhưng manual import cho types**
- **VRISTO theme patterns với Tailwind CSS**
- **Mobile-first responsive design**
- **Dark mode support**
- **RTL support**
- **Internationalization với @nuxtjs/i18n**
- **Complex filtering system với FilterRequest structure để khớp với backend**

## Learnings và project insights
- **Nuxt 3 auto-import có thể gây conflicts với types, cần cấu hình cẩn thận**
- **Tailwind CSS v4 beta còn nhiều issues, nên dùng v3 stable**
- **Third-party libraries trong VRISTO template có thể thiếu type definitions**
- **VRISTO theme có custom CSS classes (bg-success, text-white-dark, etc.) được định nghĩa trong tailwind.css**
- **Cần import đúng thứ tự: main.css → tailwind.css → @tailwind directives + custom classes**
- **Runtime config là cách tốt nhất để manage API endpoints trong Nuxt**
- **Frontend và backend cần đồng bộ chính xác về data structures, đặc biệt FilterBodyRequest và response format**
- **Backend .NET Core sử dụng IBasePaging<T> với properties: Data, Pagination**
- **Pagination object có: PageIndex, PageSize, TotalRow, PageCount (không phải pageNumber, totalRecords)**
- **PostgreSQL với Npgsql yêu cầu DateTime có Kind=UTC, cần EnableLegacyTimestampBehavior() để tự động convert**

## Bước tiếp theo
- **✅ HOÀN THÀNH: Setup và cấu hình Nuxt frontend thành công**
- **✅ HOÀN THÀNH: Fix FilterBodyRequest format mismatch**
- **Test các trang Account management với .NET API thực tế**
- **Re-enable i18n khi có compatible version**
- **Enable TypeScript strict checking sau khi fix third-party library types**
- **Implement error handling và loading states tốt hơn**
- **Thêm validation cho forms**
- **Optimize performance và SEO**
- **Implement authentication/authorization nếu cần**

## Đảm bảo mọi service có health check, logging, monitoring, alerting tự động.
- **Đã cấu hình Dependency Injection cho Unit of Work, RecurringTransactionTemplateService và ExpectedTransactionService trong ServiceExtensions.**
- **Đã tạo các validator bằng FluentValidation cho các request DTOs liên quan đến RecurringTransactionTemplate và ExpectedTransaction, và đăng ký chúng tập trung bằng extension method AddApplicationValidators.**
- **Đã cập nhật DataAnnotations cho tất cả Entity trong CoreFinance.Domain:**
  - **Thêm [Column(TypeName = "decimal(18,2)")] cho các property decimal liên quan đến tiền**
  - **Thêm [Range] validation cho các giá trị số**
  - **Thêm [MaxLength] cho các string properties**
  - **Thêm [Required] cho các property bắt buộc**
- **Đã thay đổi Foreign Key design pattern: Chuyển tất cả Foreign Keys từ Guid sang Guid? (nullable) để tạo mối quan hệ linh hoạt hơn:**
  - **Transaction: AccountId, UserId → Guid?**
  - **RecurringTransactionTemplate: UserId, AccountId → Guid?**
  - **ExpectedTransaction: RecurringTransactionTemplateId, UserId, AccountId → Guid?**
  - **Mục đích: Tăng tính linh hoạt, hỗ trợ import dữ liệu không đầy đủ, soft delete, orphaned records management**
- **Đã tổ chức lại cấu trúc unit tests theo pattern mới:**
  - **Sử dụng partial class cho mỗi service test (AccountServiceTests, ExpectedTransactionServiceTests, RecurringTransactionTemplateServiceTests).**
  - **Mỗi method của service có file test riêng theo format ServiceNameTests.MethodName.cs.**
  - **Tất cả test files được tổ chức trong thư mục con theo tên service.**
  - **Di chuyển helper functions vào TestHelpers.cs trong thư mục Helpers.**
- **Đã viết comprehensive unit tests cho tất cả methods của cả hai services mới.**
- **Đã chuẩn hóa sử dụng Bogus cho fake data trong unit test, tuân thủ .NET rule.**
- **Chỉ sử dụng xUnit cho unit test, không dùng NUnit hay framework khác.**
- **Chuẩn hóa sử dụng FluentAssertions cho assert kết quả trong unit test, tuân thủ .NET rule.**
- **Quy ước sử dụng instance AutoMapper thật (không mock) cho unit test ở tầng service, dựa vào các AutoMapper profile đã được cấu hình đúng và đã được test riêng.**
- **Đã chuẩn hóa việc đăng ký validator bằng extension method AddApplicationValidators để dễ quản lý.**
- **Lưu ý về việc đồng bộ dữ liệu giữa giao dịch dự kiến (ExpectedTransaction) và giao dịch thực tế (Transaction) thông qua ActualTransactionId khi confirm expected transaction.**