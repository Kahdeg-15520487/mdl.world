@echo off
REM Fantasy World Generator API Test Runner
REM Quick test script for Windows

echo ========================================
echo Fantasy World Generator API Test Suite
echo ========================================
echo.

REM Get port from launchSettings.json using PowerShell
echo [0/5] Reading API port from launchSettings.json...
for /f %%i in ('powershell -Command "(Get-Content '..\mdl.world\Properties\launchSettings.json' | ConvertFrom-Json).profiles.http.applicationUrl -replace 'http://localhost:', ''"') do set API_PORT=%%i

if "%API_PORT%"=="" (
    echo Warning: Could not read port from launchSettings.json, using default 5000
    set API_PORT=5000
)

set BASE_URL=http://localhost:%API_PORT%
echo Using API endpoint: %BASE_URL%
echo.

REM Check if server is running
echo [1/5] Checking if API server is running...
curl -s -o nul -w "HTTP Status: %%{http_code}" %BASE_URL%/world/themes
if %errorlevel% neq 0 (
    echo ERROR: API server is not running on %BASE_URL%
    echo Please start the server with: dotnet run
    pause
    exit /b 1
)
echo Server is running!
echo.

REM Test 1: Get themes
echo [2/5] Testing: Get Available Themes
curl -X GET %BASE_URL%/world/themes
echo.
echo.

REM Test 2: Basic world generation
echo [3/5] Testing: Basic World Generation
curl -X POST %BASE_URL%/world/generate ^
     -H "Content-Type: application/json" ^
     -d "{\"worldName\":\"BatchTestWorld\",\"theme\":\"Fantasy-SciFi\",\"techLevel\":6,\"magicLevel\":7}"
echo.
echo.

REM Test 3: Get all worlds
echo [4/5] Testing: Get All Worlds
curl -X GET %BASE_URL%/world
echo.
echo.

REM Test 4: Templates
echo [5/5] Testing: Get World Templates
curl -X GET %BASE_URL%/world/templates
echo.
echo.

echo ========================================
echo Basic API tests completed!
echo For comprehensive testing, run:
echo   PowerShell: .\test-api-suite.ps1
echo   HTTP Tests: Use test-api-collection.http
echo ========================================
pause
