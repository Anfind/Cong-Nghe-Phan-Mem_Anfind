@echo off
echo ==========================================
echo Starting JCertPre Backend Server
echo ==========================================
echo.

echo Checking .NET installation...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed or not in PATH
    echo Please install .NET 8.0 SDK and try again
    echo Download: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo .NET SDK found. Starting backend...
echo.

cd "c:\An\Cong-Nghe-Phan-Mem_Anfind\backend\JCertPreBackend"

echo Restoring packages...
dotnet restore

if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo Building project...
dotnet build

if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo ==========================================
echo Backend server starting...
echo ==========================================
echo API URL: http://localhost:5032
echo Swagger: http://localhost:5032/swagger
echo Database: Check appsettings.json for configuration
echo.
echo Press Ctrl+C to stop the server
echo ==========================================
echo.

dotnet run

pause
