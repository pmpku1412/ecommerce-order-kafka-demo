# E-commerce Order Processing - Automation Tests

ระบบ automation tests สำหรับ E-commerce Order Processing ที่ใช้ Kafka สำหรับ event-driven architecture

## โครงสร้าง Test Projects

```
tests/
├── TQM.BackOffice.UnitTests/           # Unit Tests
├── TQM.BackOffice.IntegrationTests/    # Integration Tests  
├── TQM.BackOffice.E2ETests/            # End-to-End & Performance Tests
└── README.md                           # เอกสารนี้
```

## ประเภทของ Tests

### 1. Unit Tests
- **จุดประสงค์**: ทดสอบ business logic แต่ละส่วนแยกจากกัน
- **ครอบคลุม**: Commands, Queries, Services
- **เครื่องมือ**: xUnit, Moq, FluentAssertions, AutoFixture
- **ระยะเวลา**: รวดเร็ว (< 1 นาที)

**Test Cases:**
- `CreateOrderCommandTests`: ทดสอบการสร้าง order
- `GetOrderQueryTests`: ทดสอบการดึงข้อมูล order
- `OrderServiceTests`: ทดสอบ business logic ของ OrderService

### 2. Integration Tests
- **จุดประสงค์**: ทดสอบการทำงานร่วมกันของ components
- **ครอบคลุม**: API endpoints, Kafka integration
- **เครื่องมือ**: ASP.NET Core Testing, Testcontainers
- **ระยะเวลา**: ปานกลาง (2-5 นาที)

**Test Cases:**
- `OrderControllerTests`: ทดสอบ API endpoints
- `KafkaIntegrationTests`: ทดสอบ Kafka producer/consumer

### 3. End-to-End Tests
- **จุดประสงค์**: ทดสอบ user scenarios แบบครบวงจร
- **ครอบคลุม**: Complete order flow, Error handling
- **เครื่องมือ**: WebApplicationFactory, Testcontainers
- **ระยะเวลา**: ช้า (5-10 นาที)

**Test Cases:**
- `OrderProcessingE2ETests`: ทดสอบ complete order processing flow

### 4. Performance Tests
- **จุดประสงค์**: ทดสอบประสิทธิภาพและความทนทาน
- **ครอบคลุม**: Load testing, Stress testing, Spike testing
- **เครื่องมือ**: NBomber
- **ระยะเวลา**: นาน (10-30 นาที)

**Test Cases:**
- Load Testing: ทดสอบภาระงานปกติ
- Stress Testing: หาจุดแตกหัก
- Spike Testing: ทดสอบการรับมือกับ traffic ที่เพิ่มขึ้นอย่างกะทันหัน
- Endurance Testing: ทดสอบความทนทานระยะยาว

## การรัน Tests

### Prerequisites
- .NET 6 SDK
- Docker Desktop
- Visual Studio 2022 หรือ VS Code

### รัน Tests แต่ละประเภท

#### Unit Tests
```bash
# รันผ่าน script
scripts\run-unit-tests.bat

# รันผ่าน dotnet CLI
dotnet test tests/TQM.BackOffice.UnitTests/
```

#### Integration Tests
```bash
# รันผ่าน script (รวม Kafka setup)
scripts\run-integration-tests.bat

# รันผ่าน dotnet CLI (ต้อง setup Kafka เอง)
dotnet test tests/TQM.BackOffice.IntegrationTests/
```

#### End-to-End Tests
```bash
# รันผ่าน script (รวม infrastructure setup)
scripts\run-e2e-tests.bat

# รันผ่าน dotnet CLI (ต้อง setup infrastructure เอง)
dotnet test tests/TQM.BackOffice.E2ETests/ --filter "Category!=Performance"
```

#### Performance Tests
```bash
# รันผ่าน script
scripts\run-performance-tests.bat

# รันผ่าน dotnet CLI
dotnet test tests/TQM.BackOffice.E2ETests/ --filter "FullyQualifiedName~PerformanceTests"
```

#### รัน Tests ทั้งหมด
```bash
# รันผ่าน script (แนะนำ)
scripts\run-all-tests.bat

# รันแต่ละประเภทตามลำดับ
scripts\run-unit-tests.bat
scripts\run-integration-tests.bat
scripts\run-e2e-tests.bat
scripts\run-performance-tests.bat
```

## Test Scenarios

### Order Processing Flow
1. **สร้าง Order สำเร็จ**
   - ส่ง request ที่ถูกต้อง
   - ตรวจสอบ response
   - ยืนยัน Kafka events

2. **Error Handling**
   - Product ไม่มีอยู่
   - Stock ไม่เพียงพอ
   - ข้อมูล customer ไม่ถูกต้อง

3. **Kafka Integration**
   - Event publishing
   - Event consuming
   - Message ordering
   - Error handling

4. **Performance Scenarios**
   - Normal load (10 req/sec)
   - High load (100 req/sec)
   - Spike load (sudden increase)
   - Sustained load (long duration)

## Test Data

### Mock Products
```json
[
  {"Id": "PROD001", "Name": "Laptop", "Price": 25000, "Stock": 100},
  {"Id": "PROD002", "Name": "Mouse", "Price": 500, "Stock": 200},
  {"Id": "PROD003", "Name": "Keyboard", "Price": 1500, "Stock": 150},
  {"Id": "PROD004", "Name": "Monitor", "Price": 8000, "Stock": 80},
  {"Id": "PROD005", "Name": "Headphones", "Price": 2000, "Stock": 120}
]
```

### Sample Order Request
```json
{
  "customerId": "CUST001",
  "customerName": "John Doe",
  "items": [
    {
      "productId": "PROD001",
      "quantity": 2,
      "unitPrice": 25000.00
    }
  ]
}
```

## Infrastructure Requirements

### Kafka Setup
- **Zookeeper**: Port 2181
- **Kafka Broker**: Port 9092
- **Kafka UI**: Port 8085
- **Topics**: order-events, stock-events, notification-events

### Application Setup
- **API Server**: Port 5000 (HTTP), 5001 (HTTPS)
- **Environment**: Development/Testing

## Continuous Integration

### GitHub Actions (ตัวอย่าง)
```yaml
name: Automated Tests

on: [push, pull_request]

jobs:
  unit-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 6.0.x
      - name: Run Unit Tests
        run: dotnet test tests/TQM.BackOffice.UnitTests/

  integration-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 6.0.x
      - name: Start Kafka
        run: docker-compose up -d
      - name: Run Integration Tests
        run: dotnet test tests/TQM.BackOffice.IntegrationTests/
      - name: Stop Kafka
        run: docker-compose down
```

## Best Practices

### Test Organization
1. **AAA Pattern**: Arrange, Act, Assert
2. **Descriptive Names**: ใช้ชื่อที่บอกถึงสิ่งที่ทดสอบ
3. **Single Responsibility**: แต่ละ test ทดสอบสิ่งเดียว
4. **Independent Tests**: tests ไม่ขึ้นต่อกัน

### Test Data Management
1. **Mock Data**: ใช้ข้อมูลจำลองสำหรับ unit tests
2. **Test Containers**: ใช้ containers สำหรับ integration tests
3. **Data Cleanup**: ทำความสะอาดข้อมูลหลัง tests

### Performance Testing
1. **Baseline**: กำหนด baseline performance
2. **Gradual Load**: เพิ่มภาระงานทีละน้อย
3. **Monitor Resources**: ติดตาม CPU, Memory, Network
4. **Report Results**: บันทึกผลการทดสอบ

## Troubleshooting

### Common Issues

#### Kafka Connection Failed
```
Error: Connection to Kafka failed
Solution: ตรวจสอบว่า Kafka container ทำงานอยู่
Command: docker ps | grep kafka
```

#### Port Already in Use
```
Error: Port 5000 is already in use
Solution: หยุด process ที่ใช้ port หรือเปลี่ยน port
Command: netstat -ano | findstr :5000
```

#### Test Timeout
```
Error: Test execution timeout
Solution: เพิ่ม timeout หรือตรวจสอบ infrastructure
```

### Debug Tips
1. **Verbose Logging**: เปิด detailed logging
2. **Step-by-step**: รัน tests ทีละขั้นตอน
3. **Infrastructure Check**: ตรวจสอบ Docker containers
4. **Network Issues**: ตรวจสอบ port และ firewall

## Metrics และ Reports

### Test Coverage
- **Target**: > 80% code coverage
- **Tool**: Coverlet
- **Report**: HTML และ XML format

### Performance Metrics
- **Response Time**: < 200ms (95th percentile)
- **Throughput**: > 100 req/sec
- **Error Rate**: < 1%
- **Resource Usage**: < 80% CPU/Memory

### Test Reports
- **xUnit**: XML และ HTML reports
- **NBomber**: Performance reports
- **Coverage**: Code coverage reports

## การพัฒนา Tests ใหม่

### เพิ่ม Unit Test
1. สร้างไฟล์ใน `tests/TQM.BackOffice.UnitTests/`
2. ใช้ naming convention: `{ClassName}Tests.cs`
3. เขียน test methods ตาม AAA pattern
4. รัน tests เพื่อยืนยัน

### เพิ่ม Integration Test
1. สร้างไฟล์ใน `tests/TQM.BackOffice.IntegrationTests/`
2. ใช้ `WebApplicationFactory` สำหรับ API tests
3. ใช้ `Testcontainers` สำหรับ external dependencies
4. ทดสอบ end-to-end scenarios

### เพิ่ม Performance Test
1. สร้างไฟล์ใน `tests/TQM.BackOffice.E2ETests/`
2. ใช้ NBomber framework
3. กำหนด load scenarios
4. วัดและรายงานผล

## สรุป

ระบบ automation tests นี้ครอบคลุมการทดสอบทุกระดับ ตั้งแต่ unit tests ไปจนถึง performance tests เพื่อให้มั่นใจในคุณภาพและประสิทธิภาพของระบบ E-commerce Order Processing ที่ใช้ Kafka

สำหรับคำถามหรือปัญหาเพิ่มเติม กรุณาติดต่อทีมพัฒนา
