@echo off
echo.
echo ============================================================
echo   JCertPre - Start with MySQL
echo ============================================================
echo.

echo Quick MySQL check...
mysql -u root -p210506 -e "SELECT 'OK' as Status;" >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ❌ MySQL connection failed!
    echo.
    echo Run setup first: setup-mysql.bat
    echo Or check status: check-mysql.bat
    pause
    exit /b 1
)

echo ✅ MySQL ready
echo.
echo Starting JCertPre backend...
cd backend\JCertPreBackend
dotnet run

echo.
echo Application stopped.
pause
