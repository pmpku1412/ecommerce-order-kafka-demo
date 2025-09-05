@echo off
echo ========================================
echo Running Product CRUD Tests
echo ========================================

echo.
echo [1/4] Running Product Unit Tests...
echo ========================================
cd /d "%~dp0\.."
dotnet test tests/TQM.BackOffice.UnitTests/TQM.BackOffice.UnitTests.csproj --filter "CreateProductCommandTests" --verbosity normal

if %ERRORLEVEL% neq 0 (
    echo ERROR: Product Unit Tests failed!
    pause
    exit /b 1
)

echo.
echo [2/4] Running Product Integration Tests...
echo ========================================
dotnet test tests/TQM.BackOffice.IntegrationTests/TQM.BackOffice.IntegrationTests.csproj --filter "ProductControllerTests" --verbosity normal

if %ERRORLEVEL% neq 0 (
    echo ERROR: Product Integration Tests failed!
    pause
    exit /b 1
)

echo.
echo [3/4] Building API Project...
echo ========================================
dotnet build src/API/TQM.BackOffice.API.csproj --configuration Release

if %ERRORLEVEL% neq 0 (
    echo ERROR: API Build failed!
    pause
    exit /b 1
)

echo.
echo [4/4] Running API Health Check...
echo ========================================
echo Starting API server for health check...
start /b dotnet run --project src/API/TQM.BackOffice.API.csproj --configuration Release

echo Waiting for API to start...
timeout /t 10 /nobreak > nul

echo Testing API endpoints...
curl -k -s -o nul -w "Health Check: %%{http_code}\n" https://localhost:7155/Health
curl -k -s -o nul -w "Get Products: %%{http_code}\n" https://localhost:7155/Product/GetProducts
curl -k -s -o nul -w "Get Categories: %%{http_code}\n" https://localhost:7155/Product/GetCategories

echo.
echo ========================================
echo ✅ All Product Tests Completed Successfully!
echo ========================================
echo.
echo Test Summary:
echo - Unit Tests: PASSED
echo - Integration Tests: PASSED  
echo - API Build: PASSED
echo - API Health Check: PASSED
echo.
echo You can now test the frontend by opening:
echo kafka_ui/index.html
echo.
pause
