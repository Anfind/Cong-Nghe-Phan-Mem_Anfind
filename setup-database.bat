@echo off
echo ==========================================
echo JCertPre Database Setup Script
echo ==========================================
echo.

REM Check if MySQL is installed
mysql --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: MySQL is not installed or not in PATH
    echo Please install MySQL and try again
    pause
    exit /b 1
)

echo MySQL found. Setting up database...
echo.

REM Default MySQL credentials for JCertPre
set MYSQL_USER=root
set MYSQL_PASSWORD=210506

echo Using MySQL credentials: %MYSQL_USER%/***
echo.
echo Connecting to MySQL and running setup script...

REM Run the setup script
mysql -u %MYSQL_USER% -p%MYSQL_PASSWORD% < "Migrations\001_InitialSetup.sql"

if %errorlevel% equ 0 (
    echo.
    echo ==========================================
    echo Database setup completed successfully!
    echo ==========================================
    echo.
    echo Database: jcertpre_db
    echo Default admin: admin / admin123
    echo Default user: testuser / password123
    echo.
    echo Don't forget to update appsettings.json with your MySQL credentials:
    echo "Server=localhost;Database=jcertpre_db;Uid=%MYSQL_USER%;Pwd=%MYSQL_PASSWORD%;"
    echo.
) else (
    echo.
    echo ==========================================
    echo ERROR: Database setup failed!
    echo ==========================================
    echo Please check your MySQL credentials and try again.
    echo.
)

pause
