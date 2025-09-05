@echo off
echo ========================================
echo Running All Automated Tests
echo ========================================
echo This script will run all test suites:
echo 1. Unit Tests
echo 2. Integration Tests  
echo 3. End-to-End Tests
echo 4. Performance Tests (Optional)
echo.

set /p INCLUDE_PERFORMANCE="Include Performance Tests? (y/n): "

echo.
echo ========================================
echo PHASE 1: Unit Tests
echo ========================================
call scripts\run-unit-tests.bat

if %ERRORLEVEL% neq 0 (
    echo Unit tests failed! Stopping test execution.
    pause
    exit /b 1
)

echo.
echo ========================================
echo PHASE 2: Integration Tests
echo ========================================
call scripts\run-integration-tests.bat

if %ERRORLEVEL% neq 0 (
    echo Integration tests failed! Stopping test execution.
    pause
    exit /b 1
)

echo.
echo ========================================
echo PHASE 3: End-to-End Tests
echo ========================================
call scripts\run-e2e-tests.bat

if %ERRORLEVEL% neq 0 (
    echo E2E tests failed! Stopping test execution.
    pause
    exit /b 1
)

if /i "%INCLUDE_PERFORMANCE%"=="y" (
    echo.
    echo ========================================
    echo PHASE 4: Performance Tests
    echo ========================================
    call scripts\run-performance-tests.bat
    
    if %ERRORLEVEL% neq 0 (
        echo Performance tests failed!
        pause
        exit /b 1
    )
)

echo.
echo ========================================
echo ALL TESTS COMPLETED SUCCESSFULLY!
echo ========================================
echo Test Summary:
echo ✓ Unit Tests - PASSED
echo ✓ Integration Tests - PASSED  
echo ✓ End-to-End Tests - PASSED
if /i "%INCLUDE_PERFORMANCE%"=="y" (
    echo ✓ Performance Tests - PASSED
)
echo.
echo All automation tests have been executed successfully.
echo Check individual test reports for detailed results.
echo ========================================
pause
