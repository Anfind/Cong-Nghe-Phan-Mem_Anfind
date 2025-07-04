@echo off
echo.
echo ============================================================
echo   JCertPre - MySQL Setup (Password: 210506)
echo ============================================================
echo.

echo [1/3] Testing MySQL connection...
mysql -u root -p210506 -e "SELECT 'MySQL Connected Successfully!' as Status;" 2>nul
if %ERRORLEVEL% neq 0 (
    echo ❌ ERROR: Cannot connect to MySQL
    echo.
    echo Please check:
    echo   1. MySQL Server is running: net start mysql80
    echo   2. Password is correct: 210506
    echo   3. MySQL is installed and accessible
    echo.
    pause
    exit /b 1
)
echo ✅ MySQL connection successful!

echo.
echo [2/3] Creating databases...
mysql -u root -p210506 -e "CREATE DATABASE IF NOT EXISTS jcertpre_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
mysql -u root -p210506 -e "CREATE DATABASE IF NOT EXISTS jcertpre_dev_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
echo ✅ Databases created: jcertpre_db, jcertpre_dev_db

echo.
echo [3/3] Testing application build...
cd backend\JCertPreBackend
dotnet build --verbosity quiet
if %ERRORLEVEL% neq 0 (
    echo ❌ Build failed
    pause
    exit /b 1
)
echo ✅ Application build successful!

echo.
echo ============================================================
echo   🎉 MySQL Setup Complete!
echo ============================================================
echo.
echo ✅ Configuration:
echo   - Database Provider: MySQL
echo   - Server: localhost:3306
echo   - Username: root
echo   - Password: 210506
echo   - Dev Database: jcertpre_dev_db
echo   - Prod Database: jcertpre_db
echo.
echo 🚀 Ready to start:
echo   1. dotnet run (tables will be auto-created)
echo   2. Browse: http://localhost:5032/swagger
echo   3. Login: admin/admin123 or testuser/password123
echo.
echo Starting application...
dotnet run

pause
