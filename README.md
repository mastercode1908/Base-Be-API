# 🚀 Hướng Dẫn Phát Triển và Kiểm Thử Backend API (BaseApi)

Tài liệu này hướng dẫn cách cấu hình, chạy dự án, phát triển thêm chức năng mới (Code Flow), và cách kiểm thử (Test API) dành cho các thành viên trong nhóm phát triển.

---

## 📂 1. Cấu Trúc Thư Mục Dự Án (Project Structure)

Dự án được xây dựng theo kiến trúc phân lớp chuẩn của ASP.NET Core Web API:

```text
BaseApi/
├── 📁 Configurations/     # Cấu hình hệ thống (ví dụ: JwtSettings)
├── 📁 Controllers/        # Lớp tiếp nhận Request, điều phối và trả về HTTP Response
├── 📁 Data/               # Lớp tương tác DB (EF Core - AppDbContext)
├── 📁 DTOs/               # Data Transfer Objects (Requests & Responses) để nhận/xuất dữ liệu
├── 📁 Migrations/         # Quản lý lịch sử và đồng bộ cấu trúc Database
├── 📁 Models/             # Các thực thể (Entities) ánh xạ trực tiếp xuống bảng Database
├── 📁 Repositories/       # Lớp truy xuất dữ liệu (Data Access Layer - Interfaces & Implementations)
├── 📁 Services/           # Lớp chứa logic nghiệp vụ (Business Logic Layer - Interfaces & Implementations)
├── 📄 Program.cs          # Đăng ký Service DI, cấu hình Middleware & khởi chạy API
├── 📄 appsettings.json    # File cấu hình chung (Connection String, JWT Keys, Logs)
└── 📄 README.md           # Tài liệu hướng dẫn (File này)
```

---

## 🛠️ 2. Hướng Dẫn Cài Đặt & Chạy Dự Án (Getting Started)

### Bước 1: Cấu hình Connection String
Mở file [appsettings.json](appsettings.json) và điều chỉnh chuỗi kết nối SQL Server của bạn tại mục `"DefaultConnection"`. Ví dụ:
```json
"ConnectionStrings": {
  "DefaultConnection": "server=(local);database=BaseApiDb;uid=sa;pwd=123;TrustServerCertificate=True;Trusted_Connection=True;"
}
```

### Bước 2: Tạo và Cập nhật Database (EF Core Migrations)
Mở terminal tại thư mục gốc của dự án `BaseApi` và chạy lệnh sau để cập nhật cấu trúc database xuống SQL Server:
```bash
dotnet ef database update
```
*(Nếu chưa cài đặt công cụ EF CLI, chạy lệnh `dotnet tool install --global dotnet-ef` trước).*

### Bước 3: Chạy ứng dụng
Chạy ứng dụng bằng lệnh:
```bash
dotnet run
```
Sau khi ứng dụng khởi động thành công, console sẽ hiển thị địa chỉ chạy local (mặc định là `http://localhost:5130` hoặc `https://localhost:7123`).

---

## 💻 3. Hướng Dẫn Viết Code Thêm Chức Năng (Developer Guide)

Để thêm một thực thể hoặc chức năng mới vào hệ thống (ví dụ: thực thể `Product`), hãy tuân thủ quy trình **6 bước chuẩn** sau:

### Lớp 1: Tạo Entity Model
Tạo file class trong thư mục `Models` (ví dụ `Models/Product.cs`):
```csharp
namespace BaseApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
```

### Lớp 2: Đăng ký trong DbContext & Migration
Mở file `Data/AppDbContext.cs` và khai báo `DbSet` mới:
```csharp
public DbSet<Product> Products { get; set; }
```
Sau đó tạo migration mới và update database qua terminal:
```bash
dotnet ef migrations add AddProductTable
dotnet ef database update
```

### Lớp 3: Tạo DTOs (Data Transfer Objects)
Không nên nhận trực tiếp Entity từ client hoặc trả thẳng Entity ra ngoài. Hãy tạo DTO trong thư mục `DTOs`:
- `DTOs/Requests/CreateProductRequest.cs` (Nhận thông tin khi tạo mới)
- `DTOs/Response/ProductResponse.cs` (Trả kết quả ra ngoài)

### Lớp 4: Lớp Repository (Data Access)
- Định nghĩa Interface trong `Repositories/Interfaces/IProductRepository.cs`.
- Triển khai cụ thể trong `Repositories/Implementations/ProductRepository.cs` để gọi CRUD với EF Core qua `AppDbContext`.

### Lớp 5: Lớp Service (Business Logic)
- Định nghĩa Interface trong `Services/Interfaces/IProductService.cs`.
- Triển khai cụ thể trong `Services/Implementations/ProductService.cs` (Đây là nơi xử lý các nghiệp vụ tính toán, logic kiểm tra).

### Lớp 6: Lớp Controller (API Endpoints)
Tạo Controller mới kế thừa `ControllerBase` trong thư mục `Controllers` (ví dụ `ProductController.cs`). 
Sử dụng chuẩn phản hồi đồng nhất `ApiResponse<T>` để trả dữ liệu về:
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    // ... Constructor Injection ...
}
```

*Đừng quên đăng ký các Repository và Service mới trong file `Program.cs` thông qua `builder.Services.AddScoped<IProductRepository, ProductRepository>();`*

---

## 🔒 4. Hướng Dẫn Phân Quyền Bằng JWT (Authorization & Roles)

Hệ thống đã cấu hình phân quyền đầy đủ dựa trên vai trò (Role). Có 2 Role chính được sử dụng: `"Admin"` và `"User"`.

### Các Attribute phân quyền phổ biến:

1. **`[Authorize]` (Ở cấp Controller hoặc Action):**
   Yêu cầu Client phải đăng nhập (phải đính kèm Token hợp lệ trong Header) mới có thể truy cập.
   ```csharp
   [Authorize]
   [Route("api/[controller]")]
   public class UsersController : ControllerBase
   ```

2. **`[Authorize(Roles = "Admin")]`:**
   Chỉ những tài khoản có vai trò là **Admin** mới có quyền truy cập endpoint này.
   ```csharp
   [HttpGet]
   [Authorize(Roles = "Admin")]
   public async Task<IActionResult> GetAll()
   ```

3. **`[Authorize(Roles = "Admin,User")]`:**
   Cho phép tài khoản có vai trò **Admin** HOẶC **User** truy cập.
   ```csharp
   [HttpGet("{id}")]
   [Authorize(Roles = "Admin,User")]
   public async Task<IActionResult> GetById(int id)
   ```

4. **`[AllowAnonymous]`:**
   Cho phép tất cả mọi người truy cập công khai mà không cần đăng nhập (áp dụng cho trang Đăng ký, Đăng nhập).
   ```csharp
   [HttpPost]
   [AllowAnonymous]
   public async Task<IActionResult> Create(CreateUserRequest request)
   ```

---

## 🧪 5. Hướng Dẫn Kiểm Thử API (Testing Guide)

Bạn có thể kiểm thử API trực tiếp qua giao diện **Swagger UI** (mặc định tại địa chỉ `http://localhost:5130/swagger`) hoặc sử dụng công cụ **Postman**.

### Kịch Bản Kiểm Thử Chuẩn (Test Scenario):

#### Bước 1: Đăng ký tài khoản mới (Không cần đăng nhập)
- **API:** `POST /api/users`
- **Body (JSON):**
  ```json
  {
    "fullName": "Test User",
    "email": "user_test@example.com",
    "password": "MyPassword123!",
    "role": "User" // hoặc "Admin"
  }
  ```
- **Kết quả mong đợi:** Tạo thành công user có `"isActive": true`.

#### Bước 2: Đăng nhập để lấy JWT Token
- **API:** `POST /api/auth/login`
- **Body (JSON):**
  ```json
  {
    "email": "user_test@example.com",
    "password": "MyPassword123!"
  }
  ```
- **Kết quả mong đợi:** Trả về mã thành công `200 OK` kèm chuỗi `"accessToken"`. Hãy copy toàn bộ chuỗi token này.

#### Bước 3: Gửi request có đính kèm JWT Token để kiểm thử phân quyền
*   **Trên Swagger UI:**
    1. Bấm vào nút **Authorize 🔓** ở góc trên cùng bên phải.
    2. Nhập vào ô Value theo cú pháp: `Bearer <token_vừa_copy>` (Ví dụ: `Bearer eyJhbGciOiJIUzI1Ni...`).
    3. Bấm **Authorize** rồi **Close**. Lúc này bạn đã được giả lập đăng nhập thành công.
*   **Trên Postman:**
    1. Chọn tab **Authorization** của request.
    2. Tại mục Type, chọn **Bearer Token**.
    3. Paste chuỗi token đã copy vào ô **Token**.

#### Bước 4: Kiểm tra kết quả phân quyền (Role Verification)
- **Test trường hợp 401 Unauthorized:** Thực hiện gọi `GET /api/users` khi chưa đăng nhập. Kết quả mong đợi: `401 Unauthorized`.
- **Test trường hợp 403 Forbidden:** Đăng nhập bằng tài khoản có Role là `"User"` rồi gọi endpoint `GET /api/users` (chỉ dành cho Admin). Kết quả mong đợi: `403 Forbidden`.
- **Test trường hợp 200 OK (Thành công):** Đăng nhập bằng tài khoản có Role là `"Admin"` rồi gọi endpoint `GET /api/users`. Kết quả mong đợi: Nhận về danh sách toàn bộ Users cùng mã trạng thái `200 OK`.

---

Chúc cả nhóm phát triển dự án suôn sẻ! Nếu gặp khó khăn hay lỗi phát sinh, hãy liên hệ ngay với Leader hoặc viết issue nhé! 💻✨
