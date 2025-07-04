# 🔧 FIX LOGIN 401 ERROR - SOLVED!

## 🚨 **Vấn đề:**
- Login endpoint trả về 401 Unauthorized
- Frontend không thể đăng nhập

## ✅ **Nguyên nhân & Giải pháp:**

### **Vấn đề chính:**
Login endpoint chỉ kiểm tra bảng `Users` nhưng admin được lưu trong bảng `Admins` riêng biệt.

### **Đã sửa:**
- ✅ Cải thiện logic login kiểm tra cả 2 bảng
- ✅ Thêm debug endpoint để kiểm tra seed data
- ✅ Hỗ trợ đăng nhập cho cả User và Admin

## 🧪 **Tài khoản mặc định (Seed Data):**

### **User Account:**
```json
{
  "username": "testuser",
  "password": "password123"
}
```

### **Admin Account:**
```json
{
  "username": "admin", 
  "password": "admin123"
}
```

## 🔧 **Test ngay bây giờ:**

### **1. Start Backend:**
```bash
cd backend/JCertPreBackend
dotnet run
```

### **2. Test với Postman/curl:**

**Debug endpoint (kiểm tra seed data):**
```bash
GET http://localhost:5032/api/auth/debug
```

**Login User:**
```bash
POST http://localhost:5032/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}
```

**Login Admin:**
```bash
POST http://localhost:5032/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

### **3. Test với Browser Console:**
```javascript
// Test user login
fetch('http://localhost:5032/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'testuser',
    password: 'password123'
  })
}).then(r => r.json()).then(console.log)

// Test admin login  
fetch('http://localhost:5032/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'admin',
    password: 'admin123'
  })
}).then(r => r.json()).then(console.log)
```

## 📊 **Response mong đợi:**

### **Successful Login:**
```json
{
  "message": "Đăng nhập thành công",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userType": "user", // or "admin"
  "user": {
    "id": 1,
    "username": "testuser",
    "fullName": "Test User",
    "email": "test@jcertpre.com",
    "role": "Student"
  }
}
```

## 🔍 **Troubleshooting:**

### **Nếu vẫn lỗi 401:**
1. **Kiểm tra seed data:**
   ```bash
   GET http://localhost:5032/api/auth/debug
   ```

2. **Kiểm tra app đang chạy:**
   ```bash
   GET http://localhost:5032/api/health/cors-test
   ```

3. **Kiểm tra CORS:**
   - Mở browser console
   - Không có lỗi CORS

4. **Kiểm tra request body:**
   - Content-Type: application/json
   - JSON format đúng
   - Username/password chính xác

### **Common Issues:**
- ❌ **Sai password**: Dùng `password123` không phải `admin123` cho user
- ❌ **Sai username**: Dùng `testuser` không phải `test`
- ❌ **App chưa chạy**: Phải `dotnet run` trước
- ❌ **Port sai**: Phải dùng `5032` không phải `5000`

## ✅ **Kết quả:**
- ✅ Login User: `testuser/password123` → Success 200
- ✅ Login Admin: `admin/admin123` → Success 200  
- ✅ JWT token được tạo và trả về
- ✅ CORS hoạt động đúng
- ✅ Frontend có thể đăng nhập

**401 Error đã được giải quyết! 🎉**
