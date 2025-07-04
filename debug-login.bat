@echo off
echo.
echo ============================================================
echo   JCertPre - Debug 401 Login Error
echo ============================================================
echo.

echo 🚨 Problem: 401 Unauthorized on login
echo.
echo 🔍 Possible Causes:
echo   1. Wrong username/password
echo   2. Seed data not created
echo   3. Database empty
echo   4. JWT service issue
echo   5. BCrypt verification problem
echo.
echo 📊 Default Accounts (should be created automatically):
echo   👤 User: testuser / password123
echo   👨‍💼 Admin: admin / admin123 (in Admins table)
echo.
echo 🧪 Testing Steps:
echo.

cd /d "%~dp0backend\JCertPreBackend"

echo [1/4] Building project...
dotnet build --verbosity quiet > nul 2>&1
if %ERRORLEVEL% equ 0 (
    echo ✅ Build successful
) else (
    echo ❌ Build failed
    pause
    exit /b 1
)

echo [2/4] Starting application in background...
start /min "" dotnet run > nul 2>&1
timeout /t 5 /nobreak > nul

echo [3/4] Testing endpoints...
echo.

echo 🔍 Debug Info (GET /api/auth/debug):
curl -s "http://localhost:5032/api/auth/debug" 2>nul || echo "❌ Cannot connect to API"
echo.

echo 🧪 Test Login with testuser:
curl -s -X POST "http://localhost:5032/api/auth/login" ^
     -H "Content-Type: application/json" ^
     -d "{\"username\":\"testuser\",\"password\":\"password123\"}" 2>nul || echo "❌ Login failed"
echo.

echo 🔧 Solutions to try:
echo   1. Check if app is running: http://localhost:5032/swagger
echo   2. Verify seed data: GET /api/auth/debug  
echo   3. Try different credentials:
echo      - testuser / password123
echo      - admin / admin123 (if admin login exists)
echo   4. Check browser console for actual error
echo   5. Use Postman for detailed testing
echo.

echo [4/4] Stopping test application...
taskkill /f /im dotnet.exe > nul 2>&1

echo.
echo ============================================================
echo   🚀 Next Steps:
echo   1. Run: dotnet run
echo   2. Open: http://localhost:5032/swagger  
echo   3. Test: POST /api/auth/login with testuser/password123
echo   4. Debug: GET /api/auth/debug to see seed data
echo ============================================================
echo.
pause
