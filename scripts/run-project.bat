@echo off
echo Starting E-commerce Order Kafka Demo...
echo.

echo Step 1: Starting Kafka infrastructure...
call scripts\start-kafka.bat

echo.
echo Step 2: Building .NET project...
dotnet build src/API/TQM.BackOffice.API.csproj

if %ERRORLEVEL% NEQ 0 (
    echo Build failed! Please check the errors above.
    pause
    exit /b 1
)

echo.
echo Step 3: Starting .NET API...
echo API will be available at: https://localhost:7155 or http://localhost:5139
echo Swagger UI: https://localhost:7155/swagger
echo.

dotnet run --project src/API/TQM.BackOffice.API.csproj
