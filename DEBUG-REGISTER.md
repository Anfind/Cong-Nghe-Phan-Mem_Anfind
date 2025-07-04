# 🔧 DEBUG: Nút đăng ký không hoạt động

## 🎯 Vấn đề
Nút "Đăng ký" không phản hồi khi click, không có log trong console, hoặc gửi request thất bại.

## 🚨 Các nguyên nhân phổ biến

### 1. Backend không chạy
**Triệu chứng**: Lỗi `Network Error` hoặc `ERR_CONNECTION_REFUSED`
**Giải pháp**:
```bash
# Kiểm tra .NET Core có được cài đặt
dotnet --version

# Nếu chưa có, chạy mock backend
start-mock-backend.bat

# Hoặc manual
npm install
node mock-backend.js
```

### 2. CORS Error
**Triệu chứng**: Lỗi `CORS policy` trong console
**Giải pháp**: Đảm bảo backend có CORS middleware (đã được thêm)

### 3. React State không cập nhật
**Triệu chứng**: Form không reset, button không đổi trạng thái
**Kiểm tra**: Xem console có log không

### 4. Port conflict
**Triệu chứng**: Không kết nối được backend
**Giải pháp**: Đảm bảo:
- Backend: http://localhost:5032
- Frontend: http://localhost:3000

## 🧪 Các bước debug

### Bước 1: Kiểm tra backend
```bash
# Test kết nối backend
curl http://localhost:5032/api/health

# Hoặc mở trình duyệt
http://localhost:5032/api/health
```

Kết quả mong muốn:
```json
{"status":"OK","message":"Mock backend is running"}
```

### Bước 2: Test với HTML thuần
```bash
# Mở file test HTML
start test-register.html

# Điền form và nhấn nút
# Xem console logs
```

### Bước 3: Test với React component debug
```bash
# Chạy frontend
cd frontend/jcertpre-frontend
npm start

# Truy cập: http://localhost:3000/test-register
# Xem console để debug
```

### Bước 4: Kiểm tra React DevTools
1. Cài React DevTools extension
2. Mở DevTools → Components tab
3. Tìm Register component
4. Kiểm tra state và props

## 📋 Checklist debug

- [ ] Backend đang chạy tại http://localhost:5032
- [ ] Frontend đang chạy tại http://localhost:3000  
- [ ] Console không có lỗi JavaScript
- [ ] Network tab hiển thị request được gửi
- [ ] CORS headers có trong response
- [ ] Form validation không block request
- [ ] Button không bị disabled
- [ ] State được cập nhật đúng

## 🔧 Giải pháp nhanh

### Nếu .NET không hoạt động:
```bash
# Chạy mock backend
start-mock-backend.bat
```

### Nếu React có vấn đề:
```bash
# Restart development server
cd frontend/jcertpre-frontend
npm start
```

### Nếu vẫn không hoạt động:
1. Mở http://localhost:3000/test-register
2. Mở DevTools (F12) → Console tab
3. Điền form và nhấn nút
4. Xem logs để tìm lỗi cụ thể

## 📞 Liên hệ
Nếu vẫn gặp vấn đề, hãy cung cấp:
1. Screenshot console errors
2. Network tab trong DevTools
3. Thông tin môi trường (Node.js, .NET version)

---
**💡 Tip**: Luôn kiểm tra console trước khi debug code!
