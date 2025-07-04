   # JCertPre - Hệ thống học và luyện thi tiếng Nhật

## 📖 Mô tả dự án
Ứng dụng web học và luyện thi chứng chỉ tiếng Nhật (JLPT, NAT-TEST) với các tính năng:
- Đăng nhập/đăng ký người dùng
- Quản lý khóa học với video bài giảng
- Hệ thống thi thử tự động chấm điểm
- Theo dõi kết quả học tập

## 🏗️ Kiến trúc hệ thống

### Backend (.NET 8.0)
- **Framework**: ASP.NET Core Web API
- **Database**: Entity Framework Core (InMemory)
- **Port**: http://localhost:5032, https://localhost:7098

### Frontend (React 18.2.0)
- **Framework**: ReactJS với Create React App
- **Routing**: React Router v6
- **HTTP Client**: Axios
- **Port**: http://localhost:3000

## 📊 Cơ sở dữ liệu

### Models chính:
- **User**: Người dùng (id, username, password, fullName, email)
- **Course**: Khóa học (id, name, type, description)
- **Quiz**: Bài thi (id, courseId, title, timeLimit)
- **Question**: Câu hỏi (id, quizId, text, options, correctAnswer)
- **TestResult**: Kết quả thi (id, userId, quizId, score, answers)
- **UserCourse**: Đăng ký khóa học (id, userId, courseId, progress)

## 🚀 Hướng dẫn chạy

### 1. Chạy Backend
```bash
cd backend/JCertPreBackend
dotnet run
```
✅ Backend sẽ chạy tại: http://localhost:5032
📖 Swagger UI: http://localhost:5032/swagger

### 2. Chạy Frontend
```bash
cd frontend/jcertpre-frontend
npm install
npm start
```
✅ Frontend sẽ chạy tại: http://localhost:3000

### 3. Sử dụng file .bat (Windows)
- **Backend**: Double-click `start-backend.bat`
- **Frontend**: Double-click `start-frontend.bat`

## 🔐 Tài khoản test
- **Username**: test
- **Password**: 123456

## 🎯 Tính năng đã hoàn thành

### ✅ Frontend (5/5 trang)
1. **Trang đăng nhập** (`/login`)
2. **Trang đăng ký** (`/register`) 
3. **Trang chủ** (`/`) - Danh sách khóa học
4. **Trang chi tiết khóa học** (`/course/:id`) - Video player
5. **Trang thi thử** (`/quiz/:id`) - Quiz system với timer
6. **Trang kết quả** (`/results`) - Thống kê và lịch sử

### ✅ Backend APIs
1. **Authentication**:
   - `POST /api/auth/login` - Đăng nhập
   - `POST /api/auth/register` - Đăng ký

2. **Course Management**:
   - `GET /api/course` - Lấy danh sách khóa học
   - `GET /api/course/{id}` - Chi tiết khóa học
   - `POST /api/course/enroll` - Đăng ký khóa học
   - `GET /api/course/user/{userId}/enrolled` - Khóa học đã đăng ký

3. **Quiz System**:
   - `GET /api/quiz/course/{courseId}` - Lấy bài thi theo khóa học
   - `POST /api/quiz/submit` - Nộp bài thi
   - `GET /api/quiz/results/user/{userId}` - Kết quả thi của user

## 📝 Cách test hệ thống

### 1. Test Authentication
1. Mở http://localhost:3000
2. Đăng ký tài khoản mới hoặc dùng tài khoản test
3. Đăng nhập thành công → chuyển về trang chủ

### 2. Test Course Management  
1. Xem danh sách khóa học tại trang chủ
2. Click "Xem chi tiết" → xem video và thông tin khóa học
3. Click "Đăng ký" → đăng ký khóa học

### 3. Test Quiz System
1. Click "Thi thử" từ trang chủ hoặc chi tiết khóa học
2. Làm bài thi với timer 30 phút
3. Nộp bài → xem kết quả chi tiết
4. Vào "Kết quả học tập" → xem lịch sử và thống kê

### 4. Test Backend APIs (Swagger)
1. Mở http://localhost:5032/swagger
2. Test từng endpoint với dữ liệu mẫu
3. Kiểm tra response data

## 🐛 Troubleshooting

### Lỗi thường gặp:
1. **Port đã được sử dụng**: Thay đổi port trong launchSettings.json
2. **CORS error**: Kiểm tra cấu hình CORS trong Program.cs
3. **npm start fails**: Xóa node_modules và chạy `npm install` lại
4. **Database error**: InMemory DB sẽ reset khi restart server

### Debug tips:
- Kiểm tra Network tab trong Developer Tools
- Xem console logs ở cả frontend và backend
- Sử dụng Swagger UI để test API trực tiếp

## 📈 Tính năng có thể mở rộng
- Chuyển từ InMemory sang MySQL/PostgreSQL
- Thêm phân quyền Admin/User
- Upload video thật thay vì YouTube embed  
- Hệ thống notification
- Export kết quả PDF
- Social login (Google, Facebook)
- Mobile app (React Native)

## 👥 Team
- **Backend**: .NET Core API với Entity Framework
- **Frontend**: ReactJS với modern UI/UX
- **Database**: Thiết kế schema tối ưu cho e-learning
