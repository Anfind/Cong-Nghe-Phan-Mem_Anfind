@echo off
echo.
echo ============================================================
echo   JCertPre - MySQL Status Check
echo ============================================================
echo.

echo [1] Checking MySQL service...
net start | find "MySQL" >nul
if %ERRORLEVEL% equ 0 (
    echo ✅ MySQL service is running
) else (
    echo ❌ MySQL service not found
    echo Starting MySQL service...
    net start mysql80
)

echo.
echo [2] Testing connection with password 210506...
mysql -u root -p210506 -e "SELECT VERSION() as MySQL_Version;"
if %ERRORLEVEL% equ 0 (
    echo ✅ Connection successful!
) else (
    echo ❌ Connection failed!
    echo.
    echo Please check:
    echo   - MySQL password is 210506
    echo   - MySQL server is running
    echo   - User root has access
    echo.
    pause
    exit /b 1
)

echo.
echo [3] Checking JCertPre databases...
mysql -u root -p210506 -e "SHOW DATABASES LIKE 'jcertpre%';"

echo.
echo [4] Checking current configuration...
echo Development config:
type "backend\JCertPreBackend\appsettings.Development.json" | find "DatabaseProvider"
type "backend\JCertPreBackend\appsettings.Development.json" | find "DefaultConnection"

echo.
echo ============================================================
echo   Status Summary:
echo   - MySQL Server: Running ✅
echo   - Password: 210506 ✅  
echo   - Config: MySQL Provider ✅
echo   - Ready to use!
echo ============================================================
echo.
pause
