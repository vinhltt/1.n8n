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
- **✅ HOÀN THÀNH: Transaction Page với UI/UX design mới và layout chia đôi màn hình.**
- **✅ HOÀN THÀNH: Fix FilterBodyRequest format và bug filter tài khoản.**
- **✅ HOÀN THÀNH: Nâng cấp Transaction entity với DateTime và Balance auto-calculation.**
- **✅ HOÀN THÀNH: Cleanup Transaction entity - loại bỏ Direction và Amount properties dư thừa.**
- **✅ HOÀN THÀNH: Fix Transaction display bugs - Account name và DateTime format.**
- **✅ HOÀN THÀNH: Fix Account dropdown selection trong TransactionDetailPanel.**
- **✅ HOÀN THÀNH: Fix Vue readonly ref warning và transaction detail data loading.**
- **✅ HOÀN THÀNH: Fix TypeScript errors và Category selection trong TransactionDetailPanel.**

## Thay đổi gần đây

### ✅ Transaction Display Bug Fixes (Mới hoàn thành)
- **✅ Đã fix hiển thị Account name trong transaction detail:**
  - **Cập nhật useAccountsSimple.getAccountName()** trả về "Không xác định" thay vì "Unknown Account"
  - **Sử dụng getAccountName từ composable** thay vì định nghĩa lại trong component
  - **Đảm bảo consistency** trong việc hiển thị tên tài khoản trên toàn bộ ứng dụng
- **✅ Đã fix DateTime format trong transaction:**
  - **Cập nhật input type từ "date" sang "datetime-local"** để hỗ trợ cả ngày và giờ
  - **Fix form initialization** để format datetime đúng cho input (slice(0, 16))
  - **Cập nhật convertToBackendRequest** để convert datetime-local sang ISO string
  - **Cập nhật formatDate helper** để hiển thị cả ngày và giờ (dd/MM/yyyy HH:mm)
- **✅ Đã cập nhật Frontend types:**
  - **TransactionViewModel.balance** từ nullable sang required để khớp với backend
  - **TransactionCreateRequest.balance** vẫn optional vì có thể auto-calculate
  - **Loại bỏ isBalanceCalculated** property không cần thiết

### ✅ Transaction Entity Cleanup (Mới hoàn thành)
- **✅ Đã loại bỏ các properties dư thừa:**
  - **Xóa TransactionDirection enum** - không cần thiết vì đã có RevenueAmount/SpentAmount
  - **Xóa Direction property** - logic được xử lý qua RevenueAmount/SpentAmount
  - **Xóa Amount property** - dư thừa với RevenueAmount/SpentAmount có sẵn
  - **Xóa IsBalanceCalculated property** - không cần phân biệt vì FE chỉ hiển thị, BE tự động tính
- **✅ Đã loại bỏ API calculate-balance:**
  - **Xóa CalculateBalanceRequest/Response DTOs** - không cần API riêng
  - **Xóa API endpoints** calculate-balance và latest-balance
  - **Balance calculation chỉ thực hiện trong CreateAsync/UpdateAsync** của TransactionService
- **✅ Đã giữ nguyên cấu trúc ban đầu:**
  - **RevenueAmount và SpentAmount** - cấu trúc gốc được giữ lại
  - **Balance auto-calculation** - vẫn hoạt động trong create/update transaction
  - **Logic đơn giản** - không cần flag phân biệt manual vs auto calculation

### ✅ Transaction Entity Enhancement (Đã hoàn thành trước đó)
- **✅ Đã cập nhật Transaction entity với các tính năng cần thiết:**
  - **DateTime support:** TransactionDate từ Date sang DateTime để hỗ trợ thời gian chính xác (dd/MM/yyyy HH:mm)
  - **Balance nullable:** Cho phép không nhập Balance, sẽ tự động tính dựa trên giao dịch trước
- **✅ Đã triển khai TransactionService với logic nghiệp vụ đơn giản:**
  - **CalculateBalanceForTransactionAsync (private):** Tính số dư dựa trên giao dịch gần nhất của cùng tài khoản
  - **CreateAsync override:** Tự động tính Balance nếu không được cung cấp dựa trên RevenueAmount/SpentAmount
  - **RecalculateSubsequentBalancesAsync (private):** Tính lại số dư cho tất cả giao dịch sau khi có thay đổi
- **✅ Đã cập nhật Frontend types (api.ts):**
  - **Loại bỏ TransactionDirection enum** và CalculateBalance interfaces
  - **Loại bỏ isBalanceCalculated property** - không cần thiết cho hiển thị
  - **Giữ nguyên TransactionViewModel, TransactionCreateRequest, TransactionUpdateRequest** với RevenueAmount/SpentAmount

### ✅ Transaction Design Document Update (Mới hoàn thành)
- **✅ Đã cập nhật design/screens_design/transaction.md với:**
  - **Section 4: Xử lý TransactionDate với thời gian** - DateTime picker, format dd/MM/yyyy HH:mm, validation không được chọn tương lai
  - **Section 5: Logic xử lý Balance tự động** - FE tính Balance tạm thời, BE tính Balance dựa trên giao dịch trước, cascade update
  - **API endpoints mới** cho calculate-balance và latest-balance
  - **UX Balance field** với auto-calculation, override capability, reset icon, tooltip
  - **Technical Implementation Notes** về database changes, performance, error handling

### ✅ Transaction Page Implementation (Đã hoàn thành trước đó)
- **✅ Đã cập nhật design document với layout chia đôi màn hình:**
  - **Layout responsive:** Desktop chia đôi 50/50, mobile fullscreen detail
  - **Chế độ hiển thị đơn giản:** Chỉ 4 cột chính (Ngày giờ, Mô tả, Số tiền, Số dư)
  - **Nút Columns selector:** Cho phép người dùng tùy chọn cột hiển thị
  - **Click transaction để xem detail:** Highlight transaction được chọn
  - **ESC để đóng detail pane:** Keyboard shortcut support
- **✅ Đã cập nhật pages/apps/transactions/index.vue:**
  - **Layout chia đôi màn hình** với transition animation
  - **Column visibility system** với simple/advanced modes
  - **Selected transaction highlighting** với border và background color
  - **ESC key handler** để đóng detail panel
  - **Responsive behavior** cho desktop/tablet/mobile

### ✅ Transaction Page Design (Updated)
- **Layout chia đôi màn hình:**
  - **Khi không có detail:** Danh sách chiếm toàn bộ màn hình
  - **Khi có detail:** Desktop chia đôi 50/50, mobile fullscreen overlay
  - **Transition smooth** khi mở/đóng detail panel
- **Chế độ hiển thị:**
  - **Simple mode (mặc định):** 3 cột (Ngày, Mô tả, Số tiền)
  - **Advanced mode:** Tất cả cột bao gồm Account, Category, Balance, Actions
  - **Column selector:** Dropdown với checkbox cho từng cột
  - **Nút preset:** Simple/Advanced mode switcher
- **Tương tác:**
  - **Click transaction:** Mở detail view với highlight
  - **ESC key:** Đóng detail panel
  - **Visual feedback:** Selected transaction có border trái màu primary
  - **Responsive:** Layout khác nhau cho desktop/tablet/mobile

### ✅ Technical Implementation Details
- **Layout system:** Sử dụng CSS classes với conditional rendering
- **State management:** Reactive column visibility với localStorage support
- **Keyboard events:** Global ESC listener cho close functionality
- **Visual design:** Consistent với VRISTO theme patterns
- **Performance:** Efficient re-rendering chỉ khi cần thiết

### ✅ FilterBodyRequest Format Fix (Mới hoàn thành)
- **✅ Đã cập nhật FilterDetailsRequest để khớp hoàn toàn với backend:**
  - **Data types:** `value` từ `any` → `string?` để khớp với backend
  - **Enum naming:** `filterOperator` → `FilterType` để khớp với backend
- **✅ Đã cập nhật tất cả usage trong useTransactions.ts:**
  - **Filter building logic** sử dụng property names mới
  - **Type imports** cập nhật từ FilterOperator sang FilterType
- **✅ Đã fix bug filter "Tất cả tài khoản":**
  - **Root cause:** Khi chọn "Tất cả tài khoản" (value = ""), logic merge filter không xóa accountId cũ
  - **Solution:** Thêm logic clear filter khi value là empty string/null/undefined
  - **Improved logic:** `handleAccountChange` luôn gọi `getTransactions({ accountId: value })` thay vì conditional logic
  - **Filter clearing:** Khi filter value rỗng, xóa hoàn toàn khỏi currentFilter thay vì giữ lại

### ✅ Technical Implementation Details (FilterBodyRequest Fix)
- **Property mapping:** Frontend và backend giờ đã 100% đồng bộ về naming convention
- **Filter clearing logic:** Xử lý đúng việc clear filter khi user chọn "Tất cả" options
- **Type safety:** Cập nhật exports trong types/index.ts để đảm bảo consistency
- **Backward compatibility:** Không breaking changes cho existing functionality

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

### ✅ Account Dropdown Selection Fix (Mới hoàn thành)
- **✅ Đã fix Account dropdown không select đúng giá trị:**
  - **Create mode:** Tự động chọn account từ filter hiện tại (`selectedAccountId`)
  - **View/Edit mode:** Hiển thị đúng account của transaction đó
  - **Validation:** Kiểm tra account tồn tại trong danh sách, fallback về account đầu tiên nếu không tìm thấy
  - **Reactive updates:** Form được cập nhật khi accounts load hoặc props thay đổi
- **✅ Đã cải thiện form initialization logic:**
  - **createFormDefaults:** Sử dụng datetime format đúng (slice(0, 16))
  - **Watchers:** Theo dõi thay đổi của accounts, defaultAccountId, transaction
  - **Account validation:** Đảm bảo accountId luôn hợp lệ và tồn tại trong dropdown

### ✅ Vue Readonly Ref Warning Fix (Mới hoàn thành)
- **✅ Đã fix Vue warning "Set operation on key 'value' failed: target is readonly":**
  - **Root cause:** `selectedTransaction` từ `useTransactions` được return như `readonly(selectedTransaction)`
  - **Vấn đề:** Trang chính cố gắng ghi trực tiếp vào readonly ref: `selectedTransaction.value = transaction`
  - **Giải pháp:** Thêm `setSelectedTransaction()` function trong composable để manage state properly
  - **Cập nhật:** Tất cả nơi modify selectedTransaction đều sử dụng function thay vì ghi trực tiếp
- **✅ Đã cải thiện state management:**
  - **Proper encapsulation:** State chỉ được modify thông qua dedicated functions
  - **Type safety:** Đảm bảo readonly refs không bị modify trực tiếp
  - **Clean code:** Loại bỏ debug logs và cải thiện code structure

### ✅ TypeScript Errors và Category Selection Fix (Mới hoàn thành)
- **✅ Đã fix các lỗi TypeScript:**
  - **Props interface:** `defaultDirection` từ `number` sang `TransactionDirectionType`
  - **Accounts readonly:** Sử dụng spread operator `[...accounts]` để convert readonly array
  - **CategoryType index:** Sử dụng `Record<number, string>` type annotation
  - **Import types:** Thêm `TransactionDirectionType` import
- **✅ Đã fix Category selection logic:**
  - **Auto-set categoryType:** Dựa trên transactionDirection (Revenue → Income, Spent → Expense)
  - **Reactive updates:** Watcher tự động cập nhật categoryType khi user thay đổi direction
  - **Create mode:** CategoryType được set đúng từ defaultDirection
  - **Edit/View mode:** Giữ nguyên categoryType từ transaction data
- **✅ Đã cải thiện UX:**
  - **Smart defaults:** Form tự động chọn category phù hợp với loại giao dịch
  - **Consistent behavior:** Logic nhất quán giữa create và edit modes
  - **Type safety:** Tất cả types đều chính xác và type-safe

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
- **✅ HOÀN THÀNH: Implement Transaction page trong Nuxt với thiết kế layout chia đôi**
- **🔄 TIẾP THEO: Test transaction CRUD operations với .NET API thực tế**
- **Tối ưu performance cho large transaction lists với virtual scrolling**
- **Implement advanced filtering và search functionality**
- **Thêm transaction import/export functionality**
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