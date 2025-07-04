@echo off
echo ==========================================
echo JCertPre Complete Setup Script
echo ==========================================
echo This script will set up the entire JCertPre application
echo Including database, backend, and frontend
echo.

set /p CONFIRM="Do you want to continue? (y/n): "
if /i not "%CONFIRM%"=="y" (
    echo Setup cancelled.
    pause
    exit /b 0
)

echo.
echo ==========================================
echo STEP 1: Database Setup
echo ==========================================
echo.

call setup-database.bat

if %errorlevel% neq 0 (
    echo Database setup failed. Exiting...
    pause
    exit /b 1
)

echo.
echo ==========================================
echo STEP 2: Backend Dependencies
echo ==========================================
echo.

cd "c:\An\Cong-Nghe-Phan-Mem_Anfind\backend\JCertPreBackend"

echo Checking .NET installation...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed
    echo Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo Restoring .NET packages...
dotnet restore

echo Building backend...
dotnet build

if %errorlevel% neq 0 (
    echo Backend build failed!
    pause
    exit /b 1
)

echo.
echo ==========================================
echo STEP 3: Frontend Dependencies  
echo ==========================================
echo.

cd "c:\An\Cong-Nghe-Phan-Mem_Anfind\frontend\jcertpre-frontend"

echo Checking Node.js installation...
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Node.js is not installed
    echo Please install Node.js from: https://nodejs.org
    pause
    exit /b 1
)

echo Installing npm packages...
npm install

if %errorlevel% neq 0 (
    echo Frontend dependencies installation failed!
    pause
    exit /b 1
)

echo.
echo ==========================================
echo STEP 4: Mock Backend (Optional)
echo ==========================================
echo.

cd "c:\An\Cong-Nghe-Phan-Mem_Anfind"

echo Installing mock backend dependencies...
npm install

echo.
echo ==========================================
echo SETUP COMPLETED SUCCESSFULLY!
echo ==========================================
echo.
echo To start the application:
echo.
echo 1. Database Setup: ✓ Completed
echo 2. Backend: Run 'start-backend.bat'
echo 3. Frontend: Run 'start-frontend.bat'
echo 4. Mock Backend (if needed): Run 'start-mock-backend.bat'
echo.
echo Default URLs:
echo - Backend API: http://localhost:5032
echo - Frontend: http://localhost:3000
echo - Swagger: http://localhost:5032/swagger
echo.
echo Default Accounts:
echo - Admin: admin / admin123
echo - User: testuser / password123
echo.
echo Happy coding! 🚀
echo.

pause
