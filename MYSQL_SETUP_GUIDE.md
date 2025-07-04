# 🔄 SWITCH TO MYSQL - PASSWORD: 210506

## ✅ **Configuration Updated:**

### **Files Changed:**
- ✅ `appsettings.Development.json` - Password: 210506
- ✅ `appsettings.json` - Password: 210506  
- ✅ `setup-database.bat` - Auto password
- ✅ `001_InitialSetup.sql` - Development DB added

### **Database Settings:**
```json
{
  "DatabaseProvider": "MySQL",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=jcertpre_dev_db;Uid=root;Pwd=210506;"
  }
}
```

## 🚀 **Quick Setup:**

### **Option 1: Automatic Setup**
```bash
# Run the auto-setup script
switch-to-mysql.bat
```

### **Option 2: Manual Setup**
```bash
# 1. Create database
mysql -u root -p210506 < backend/JCertPreBackend/Migrations/001_InitialSetup.sql

# 2. Build and run
cd backend/JCertPreBackend
dotnet build
dotnet run
```

### **Option 3: Quick Test**
```bash
# Test MySQL connection
mysql -u root -p210506 -e "SHOW DATABASES;"

# Should show: jcertpre_dev_db
```

## 📊 **Database Info:**

### **Databases Created:**
- **Development**: `jcertpre_dev_db` (active)
- **Production**: `jcertpre_db` 

### **Connection:**
- **Server**: localhost:3306
- **Username**: root
- **Password**: 210506

### **Seed Data:** (Auto-created when app starts)
- **Admin**: admin / admin123
- **User**: testuser / password123
- **Courses**: 2 sample courses

## 🧪 **Test Steps:**

1. **Start MySQL** (if not running):
   ```bash
   net start mysql
   ```

2. **Run setup**:
   ```bash
   switch-to-mysql.bat
   ```

3. **Start application**:
   ```bash
   cd backend/JCertPreBackend
   dotnet run
   ```

4. **Test endpoints**:
   - `GET /api/auth/debug` - Check seed data
   - `POST /api/auth/login` - Test login
   - `GET /api/course` - List courses

## ✅ **Benefits of MySQL:**

- ✅ **Persistent Data** - Data survives app restart
- ✅ **Real Database** - Production-like environment  
- ✅ **Better Performance** - Proper indexing
- ✅ **Data Inspection** - Can use MySQL Workbench
- ✅ **Backup/Restore** - Standard MySQL tools

## 🔧 **Troubleshooting:**

### **MySQL Not Running:**
```bash
net start mysql
# or
services.msc → Start MySQL80
```

### **Wrong Password:**
```bash
mysql -u root -p
# Enter: 210506
```

### **Database Not Found:**
```bash
mysql -u root -p210506 -e "CREATE DATABASE jcertpre_dev_db;"
```

---

**🎉 MySQL setup complete! App now uses persistent MySQL database with password 210506!**
