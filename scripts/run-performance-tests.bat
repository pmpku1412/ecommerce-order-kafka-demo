@echo off
echo ========================================
echo Running Performance Tests
echo ========================================

echo.
echo Starting Kafka Infrastructure...
call scripts\start-kafka.bat

echo.
echo Waiting for Kafka to be ready...
timeout /t 30 /nobreak

echo.
echo Starting Application...
start "API Server" cmd /c "dotnet run --project src/API/TQM.BackOffice.API.csproj --configuration Release"

echo.
echo Waiting for Application to start...
timeout /t 20 /nobreak

echo.
echo Building Performance Test Project...
dotnet build tests/TQM.BackOffice.E2ETests/TQM.BackOffice.E2ETests.csproj --configuration Release

if %ERRORLEVEL% neq 0 (
    echo Build failed!
    taskkill /f /im dotnet.exe 2>nul
    call scripts\stop-kafka.bat
    pause
    exit /b 1
)

echo.
echo Running Performance Tests...
echo WARNING: Performance tests may take several minutes to complete.
echo.
dotnet test tests/TQM.BackOffice.E2ETests/TQM.BackOffice.E2ETests.csproj --logger "console;verbosity=detailed" --filter "FullyQualifiedName~PerformanceTests" --configuration Release

set TEST_RESULT=%ERRORLEVEL%

echo.
echo Stopping Application...
taskkill /f /im dotnet.exe 2>nul

echo.
echo Stopping Kafka Infrastructure...
call scripts\stop-kafka.bat

if %TEST_RESULT% neq 0 (
    echo Performance tests failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Performance Tests Completed Successfully!
echo Check NBomber reports for detailed results.
echo ========================================
pause
