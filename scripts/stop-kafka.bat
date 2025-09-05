@echo off
echo Stopping Kafka infrastructure...
echo.

echo Stopping all services...
docker-compose down

echo.
echo Checking if services are stopped...
docker-compose ps

echo.
echo Kafka infrastructure stopped successfully!
echo.
echo To remove all data (volumes): docker-compose down -v
echo To start again: scripts\start-kafka.bat
