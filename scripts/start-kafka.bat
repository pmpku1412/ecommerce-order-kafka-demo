@echo off
echo Starting Kafka infrastructure...
echo.

echo Pulling latest Docker images...
docker-compose pull

echo.
echo Starting services...
docker-compose up -d

echo.
echo Waiting for services to be ready...
timeout /t 30 /nobreak > nul

echo.
echo Checking service status...
docker-compose ps

echo.
echo Kafka infrastructure is ready!
echo.
echo Services available:
echo - Kafka: localhost:9092
echo - Kafka UI: http://localhost:8085
echo - Zookeeper: localhost:2181
echo.
echo To view logs: docker-compose logs -f
echo To stop services: docker-compose down
