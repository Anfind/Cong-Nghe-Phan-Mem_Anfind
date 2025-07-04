# 🗄️ JCertPre - MySQL Setup Guide

## ✅ **Hệ thống đã được cấu hình cho MySQL:**

### **🔧 Configuration:**
- **Database Provider**: MySQL (không còn InMemory)
- **Password**: 210506  
- **Development DB**: jcertpre_dev_db
- **Production DB**: jcertpre_db
- **Connection**: root@localhost:3306

### **📁 Files Updated:**
- ✅ `appsettings.Development.json` → MySQL + password 210506
- ✅ `appsettings.json` → MySQL + password 210506
- ✅ `Program.cs` → Sẵn sàng cho MySQL

## 🚀 **Hướng dẫn sử dụng:**

### **BƯỚC 1: Setup MySQL (lần đầu)**
```bash
# Kiểm tra MySQL
check-mysql.bat

# Setup databases
setup-mysql.bat
```

### **BƯỚC 2: Chạy ứng dụng**
```bash
# Start với MySQL
start-mysql.bat

# Hoặc manual
cd backend\JCertPreBackend
dotnet run
```

### **BƯỚC 3: Test endpoints**
```bash
# Swagger UI
http://localhost:5032/swagger

# Test login
POST http://localhost:5032/api/auth/login
{
  "username": "admin",
  "password": "admin123"
}

# Check seed data  
GET http://localhost:5032/api/auth/debug
```

## 📊 **Tài khoản mặc định:**
- **Admin**: admin / admin123
- **User**: testuser / password123

## 🔍 **Troubleshooting:**

### **MySQL không kết nối được:**
```bash
# Start MySQL service
net start mysql80

# Test connection
mysql -u root -p210506

# Check services
services.msc → MySQL80
```

### **Database không tồn tại:**
```bash
# Create databases manually
mysql -u root -p210506 -e "CREATE DATABASE jcertpre_dev_db;"
mysql -u root -p210506 -e "CREATE DATABASE jcertpre_db;"
```

### **Tables không được tạo:**
- Tables sẽ được tạo tự động khi app start
- Seed data sẽ được insert tự động
- Không cần chạy migration script manual

## ✨ **Lợi ích của MySQL:**
- ✅ **Persistent data** - dữ liệu không mất khi restart
- ✅ **Real database** - môi trường giống production
- ✅ **Performance** - tốt hơn InMemory
- ✅ **Scalable** - có thể mở rộng
- ✅ **Backup/Restore** - dữ liệu an toàn

## 📋 **Quick Commands:**

```bash
# Full setup
setup-mysql.bat

# Quick start  
start-mysql.bat

# Check status
check-mysql.bat

# Manual start
cd backend\JCertPreBackend && dotnet run
```

---

**🎉 Hệ thống đã chuyển hoàn toàn sang MySQL với password 210506!**

**Chạy `setup-mysql.bat` để bắt đầu!**
