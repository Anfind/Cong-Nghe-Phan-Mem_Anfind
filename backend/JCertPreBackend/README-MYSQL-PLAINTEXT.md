# JCertPre Backend - MySQL với Plain Text Password

## Tóm tắt thay đổi
- ✅ Chuyển từ InMemory Database sang MySQL
- ✅ Sử dụng password plain text (không mã hóa)
- ✅ Sửa tất cả lỗi build JWT và IdentityModel
- ✅ Dọn dẹp các file không cần thiết
- ✅ Tạo hệ thống script quản lý MySQL

## Cấu hình hiện tại

### Database
- **Loại**: MySQL
- **Server**: localhost:3306
- **Database**: JCertPreDB
- **Username**: root
- **Password**: 210506

### Bảo mật
- **Password Storage**: Plain text (không hash)
- **JWT**: Enabled với SymmetricSecurityKey
- **CORS**: Enabled cho React frontend

## Scripts hỗ trợ

### 1. Setup và quản lý MySQL
- `setup-mysql.bat` - Tạo database và tables
- `start-mysql.bat` - Khởi động MySQL service
- `check-mysql.bat` - Kiểm tra trạng thái MySQL
- `mysql-menu.bat` - Menu tổng hợp các tác vụ MySQL

### 2. Backend management
- `final-check-and-start.bat` - Kiểm tra toàn diện và khởi động backend
- `start-backend.bat` - Khởi động nhanh backend
- `complete-setup.bat` - Setup hoàn chỉnh từ đầu

## Hướng dẫn sử dụng

### Lần đầu chạy
1. Chạy `setup-mysql.bat` để tạo database
2. Chạy `final-check-and-start.bat` để kiểm tra và khởi động

### Chạy hàng ngày
1. Chạy `start-mysql.bat` (nếu MySQL chưa chạy)
2. Chạy `start-backend.bat`

## Tài khoản mẫu

### User thường
- Username: `testuser`
- Password: `password123`
- Role: Student

### Admin
- Username: `admin`
- Password: `admin123`
- Role: Admin

## API Endpoints

### Authentication
- `POST /api/auth/login` - Đăng nhập user
- `POST /api/auth/register` - Đăng ký user mới
- `POST /api/auth/change-password` - Đổi mật khẩu

### Admin
- `POST /api/admin/login` - Đăng nhập admin
- `GET /api/admin/users` - Quản lý users
- `GET /api/admin/dashboard/stats` - Thống kê dashboard

### Courses
- `GET /api/course` - Danh sách khóa học
- `GET /api/course/{id}` - Chi tiết khóa học

## Kiểm tra hoạt động

### 1. Test API với curl
```bash
# Test health check
curl http://localhost:5000/api/health

# Test login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"testuser\",\"password\":\"password123\"}"
```

### 2. Test với browser
- Swagger UI: http://localhost:5000/swagger
- Health check: http://localhost:5000/api/health

## Troubleshooting

### Lỗi kết nối MySQL
```bash
# Kiểm tra MySQL service
net start MySQL80

# Kiểm tra port
netstat -an | findstr :3306
```

### Lỗi build
```bash
# Clean và restore
dotnet clean
dotnet restore
dotnet build
```

### Lỗi JWT
- Kiểm tra appsettings.json có đúng JwtSettings không
- Kiểm tra các package NuGet đã được install

## Ghi chú quan trọng

⚠️ **Bảo mật**: Hệ thống hiện tại sử dụng plain text password chỉ phù hợp cho môi trường development/testing.

✅ **Sẵn sàng production**: Khi cần deploy, hãy bật lại password hashing trong AuthController và AdminController.

📝 **Database**: Dữ liệu được seed tự động khi khởi động backend lần đầu.
