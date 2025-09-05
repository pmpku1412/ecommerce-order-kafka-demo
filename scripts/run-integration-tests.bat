@echo off
echo ========================================
echo Running Integration Tests
echo ========================================

echo.
echo Starting Kafka Infrastructure...
call scripts\start-kafka.bat

echo.
echo Waiting for Kafka to be ready...
timeout /t 30 /nobreak

echo.
echo Building Integration Test Project...
dotnet build tests/TQM.BackOffice.IntegrationTests/TQM.BackOffice.IntegrationTests.csproj

if %ERRORLEVEL% neq 0 (
    echo Build failed!
    call scripts\stop-kafka.bat
    pause
    exit /b 1
)

echo.
echo Running Integration Tests...
dotnet test tests/TQM.BackOffice.IntegrationTests/TQM.BackOffice.IntegrationTests.csproj --logger "console;verbosity=detailed" --collect:"XPlat Code Coverage"

set TEST_RESULT=%ERRORLEVEL%

echo.
echo Stopping Kafka Infrastructure...
call scripts\stop-kafka.bat

if %TEST_RESULT% neq 0 (
    echo Integration tests failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Integration Tests Completed Successfully!
echo ========================================
pause
