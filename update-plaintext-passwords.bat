@echo off
echo.
echo ============================================================
echo   JCertPre - Update to Plain Text Passwords
echo ============================================================
echo.

echo 🔓 Converting to plain text passwords...
echo.

echo [1/3] Updating existing database records...
mysql -u root -p210506 -e "
USE jcertpre_dev_db;
UPDATE admins SET Password = 'admin123' WHERE Username = 'admin';
UPDATE users SET Password = 'password123' WHERE Username = 'testuser';
SELECT 'Database updated' as Status;
" 2>nul

if %ERRORLEVEL% equ 0 (
    echo ✅ Database passwords updated to plain text
) else (
    echo ❌ Database update failed (maybe tables don't exist yet)
)

echo.
echo [2/3] Testing application build...
cd backend\JCertPreBackend
dotnet build --verbosity quiet
if %ERRORLEVEL% equ 0 (
    echo ✅ Application builds successfully
) else (
    echo ❌ Build failed
    pause
    exit /b 1
)

echo.
echo [3/3] Testing login functionality...
echo Starting app for quick test...
start /min "" dotnet run
timeout /t 5 /nobreak > nul

curl -s -X POST "http://localhost:5032/api/auth/login" ^
     -H "Content-Type: application/json" ^
     -d "{\"username\":\"admin\",\"password\":\"admin123\"}" > test_login.json 2>nul

if exist test_login.json (
    echo ✅ Login test completed
    type test_login.json | find "token" >nul
    if %ERRORLEVEL% equ 0 (
        echo ✅ Login successful - token generated
    ) else (
        echo ❌ Login failed - check credentials
    )
    del test_login.json
)

taskkill /f /im dotnet.exe > nul 2>&1

echo.
echo ============================================================
echo   🎉 Plain Text Password Setup Complete!
echo ============================================================
echo.
echo ✅ Changes Applied:
echo   - AuthController: No more BCrypt verification
echo   - Program.cs: Plain text seed data
echo   - SQL: Plain text passwords in database
echo.
echo 🔑 Test Accounts:
echo   - Admin: admin / admin123
echo   - User: testuser / password123
echo.
echo 🚀 Ready to use:
echo   1. dotnet run (start application)
echo   2. Test login with plain text passwords
echo   3. No password hashing complexity
echo.
cd /d "%~dp0"
pause
