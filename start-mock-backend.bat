@echo off
echo Starting Mock Backend Server...
echo.
echo Installing dependencies if needed...
npm install

echo.
echo Starting server on http://localhost:5032...
node mock-backend.js

pause
