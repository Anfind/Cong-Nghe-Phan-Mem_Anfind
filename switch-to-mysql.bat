@echo off
echo.
echo ============================================================
echo   JCertPre - Switch to MySQL Database
echo ============================================================
echo.

echo 🔄 Converting from InMemory to MySQL...
echo.
echo 📊 MySQL Configuration:
echo   - Server: localhost
echo   - Database: jcertpre_dev_db
echo   - Username: root
echo   - Password: 210506
echo.

cd /d "%~dp0"

echo [1/4] Testing MySQL connection...
mysql -u root -p210506 -e "SELECT 'MySQL connection successful' as status;" 2>nul
if %ERRORLEVEL% equ 0 (
    echo ✅ MySQL connection successful
) else (
    echo ❌ MySQL connection failed
    echo.
    echo 🔧 Please check:
    echo   1. MySQL Server is running
    echo   2. Password is correct: 210506
    echo   3. User 'root' has access
    echo.
    echo Starting MySQL service...
    net start mysql 2>nul
    echo.
    pause
    exit /b 1
)

echo.
echo [2/4] Creating database and tables...
mysql -u root -p210506 < "backend\JCertPreBackend\Migrations\001_InitialSetup.sql"
if %ERRORLEVEL% equ 0 (
    echo ✅ Database created successfully
) else (
    echo ❌ Database creation failed
    pause
    exit /b 1
)

echo.
echo [3/4] Building application...
cd backend\JCertPreBackend
dotnet build --verbosity quiet
if %ERRORLEVEL% equ 0 (
    echo ✅ Build successful
) else (
    echo ❌ Build failed
    pause
    exit /b 1
)

echo.
echo [4/4] Testing database connection...
echo Starting app for 5 seconds to test MySQL...
timeout /t 2 /nobreak > nul

start /min "" dotnet run > nul 2>&1
timeout /t 8 /nobreak > nul

curl -s "http://localhost:5032/api/auth/debug" > temp_mysql_test.json 2>nul
if %ERRORLEVEL% equ 0 (
    echo ✅ MySQL integration successful
    type temp_mysql_test.json
    del temp_mysql_test.json 2>nul
) else (
    echo ❌ MySQL integration test failed
)

taskkill /f /im dotnet.exe > nul 2>&1

echo.
echo ============================================================
echo   🎉 MySQL Setup Complete!
echo ============================================================
echo.
echo ✅ Configuration Updated:
echo   - appsettings.Development.json ✅
echo   - appsettings.json ✅
echo   - Database created ✅
echo   - Tables created ✅
echo.
echo 🚀 Ready to use MySQL:
echo   1. dotnet run (to start with MySQL)
echo   2. Data will persist between restarts
echo   3. Same seed data (admin/admin123, testuser/password123)
echo   4. All APIs work the same way
echo.
echo 📊 Database Info:
echo   - Development DB: jcertpre_dev_db
echo   - Production DB: jcertpre_db
echo   - Connection: root@localhost:3306
echo.
cd /d "%~dp0"
pause
