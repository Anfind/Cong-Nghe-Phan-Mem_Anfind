@echo off
echo ========================================
echo   JCertPre Backend - Final Check
echo ========================================
echo.

echo [1/4] Checking MySQL connection...
mysql -u root -p210506 -e "SELECT 1;" 2>nul
if %errorlevel% equ 0 (
    echo ✓ MySQL connection OK
) else (
    echo ✗ MySQL connection failed
    echo Please start MySQL service first
    pause
    exit /b 1
)

echo.
echo [2/4] Checking database...
mysql -u root -p210506 -e "USE JCertPreDB; SELECT COUNT(*) FROM users;" 2>nul
if %errorlevel% equ 0 (
    echo ✓ Database and tables exist
) else (
    echo ! Database may need to be created - will be done automatically
)

echo.
echo [3/4] Building backend project...
cd "%~dp0"
dotnet build --verbosity minimal
if %errorlevel% equ 0 (
    echo ✓ Build successful
) else (
    echo ✗ Build failed
    pause
    exit /b 1
)

echo.
echo [4/4] Ready to start backend!
echo.
echo ========================================
echo   Configuration Summary
echo ========================================
echo Database: MySQL (localhost:3306)
echo Database Name: JCertPreDB
echo Password Mode: Plain Text (no encryption)
echo JWT Authentication: Enabled
echo CORS: Enabled for React frontend
echo.
echo Sample Login Credentials:
echo - User: testuser / password123
echo - Admin: admin / admin123
echo.
echo Press any key to start the backend server...
pause >nul

echo.
echo Starting JCertPre Backend...
dotnet run

echo.
echo Backend stopped.
pause
