@echo off
echo ========================================
echo Running Unit Tests
echo ========================================

echo.
echo Building Unit Test Project...
dotnet build tests/TQM.BackOffice.UnitTests/TQM.BackOffice.UnitTests.csproj

if %ERRORLEVEL% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Running Unit Tests...
dotnet test tests/TQM.BackOffice.UnitTests/TQM.BackOffice.UnitTests.csproj --logger "console;verbosity=detailed" --collect:"XPlat Code Coverage"

if %ERRORLEVEL% neq 0 (
    echo Unit tests failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Unit Tests Completed Successfully!
echo ========================================
pause
