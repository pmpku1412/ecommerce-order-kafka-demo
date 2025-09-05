# E-commerce Order Processing with Kafka Demo

โปรเจกต์นี้เป็นตัวอย่างการใช้งาน Apache Kafka ในระบบ E-commerce Order Processing โดยใช้ .NET 6 และ Clean Architecture pattern พร้อม MediatR

## 🏗️ Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   API Layer     │    │  Application    │    │    Domain       │
│                 │    │     Layer       │    │     Layer       │
│ - OrderController│    │ - Commands      │    │ - Entities      │
│ - MasterData    │    │ - Queries       │    │ - Events        │
│   Controller    │    │ - DTOs          │    │ - Value Objects │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         └───────────────────────┼───────────────────────┘
                                 │
┌─────────────────────────────────┼─────────────────────────────────┐
│                Infrastructure Layer                                │
│                                                                   │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐   │
│  │   Persistence   │  │   Kafka         │  │   Services      │   │
│  │                 │  │                 │  │                 │   │
│  │ - OrderService  │  │ - Producer      │  │ - Background    │   │
│  │ - Mock Data     │  │ - Consumer      │  │   Services      │   │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘   │
└───────────────────────────────────────────────────────────────────┘
```

## 🚀 Features

### E-commerce Order Processing
- **สร้าง Order**: ลูกค้าสามารถสร้างคำสั่งซื้อได้
- **จัดการ Stock**: อัปเดต stock อัตโนมัติเมื่อมีการสั่งซื้อ
- **ส่งการแจ้งเตือน**: ส่งอีเมลยืนยันการสั่งซื้อ
- **Analytics**: บันทึกข้อมูลสำหรับการวิเคราะห์

### Kafka Integration
- **Producer**: ส่ง events ไปยัง Kafka topics
- **Consumer**: รับและประมวลผล events แบบ real-time
- **Topics**: 
  - `order-created`: เมื่อมีการสร้าง order ใหม่
  - `stock-updated`: เมื่อมีการอัปเดต stock
  - `notification-sent`: เมื่อส่งการแจ้งเตือนแล้ว

## 📁 Project Structure

```
src/
├── API/                              # API Layer
│   ├── Controllers/
│   │   ├── OrderController.cs        # Order API endpoints
│   │   └── MasterDataController.cs   # Master data endpoints
│   └── appsettings.Development.json  # Kafka configuration
├── Core/
│   ├── TQM.Backoffice.Application/   # Application Layer
│   │   ├── Commands/Orders/          # Order commands
│   │   ├── Queries/Orders/           # Order queries
│   │   ├── DTOs/Orders/              # Data transfer objects
│   │   └── Contracts/                # Interfaces
│   └── TQM.Backoffice.Domain/        # Domain Layer
│       ├── Entities/                 # Domain entities
│       └── Events/                   # Domain events
└── Infrastructure/
    └── TQM.BackOffice.Persistence/   # Infrastructure Layer
        └── Services/                 # Service implementations
            ├── OrderService.cs       # Order business logic
            ├── KafkaProducer.cs      # Kafka message producer
            └── KafkaConsumerService.cs # Kafka message consumer
```

## 🛠️ Setup & Installation

### Prerequisites
- .NET 6 SDK
- Docker Desktop

### Quick Start (แนะนำ)
```bash
# Clone repository
git clone <repository-url>
cd ecommerce-order-kafka-demo

# รันทั้งระบบด้วยคำสั่งเดียว
scripts\run-project.bat
```

### Manual Setup

#### 1. Clone Repository
```bash
git clone <repository-url>
cd ecommerce-order-kafka-demo
```

#### 2. Start Kafka Infrastructure
```bash
# เริ่ม Kafka และ Zookeeper
scripts\start-kafka.bat

# หรือใช้ docker-compose โดยตรง
docker-compose up -d
```

#### 3. Build & Run .NET API
```bash
# Build project
dotnet build src/API/TQM.BackOffice.API.csproj

# Run API
dotnet run --project src/API/TQM.BackOffice.API.csproj
```

#### 4. Stop Services
```bash
# หยุด Kafka infrastructure
scripts\stop-kafka.bat

# หรือใช้ docker-compose โดยตรง
docker-compose down
```

## 📝 API Usage Examples

### 1. สร้าง Order
```bash
POST /Order/CreateOrder
Content-Type: application/json

{
  "request": {
    "customerId": "CUST001",
    "customerName": "John Doe",
    "customerEmail": "john@example.com",
    "shippingAddress": "123 Main St, Bangkok",
    "paymentMethod": "Credit Card",
    "orderItems": [
      {
        "productId": "PROD001",
        "quantity": 1
      },
      {
        "productId": "PROD002",
        "quantity": 2
      }
    ]
  }
}
```

### 2. ดูข้อมูล Order
```bash
GET /Order/GetOrder/1
GET /Order/GetAllOrders
GET /Order/GetOrdersByCustomer/CUST001
```

## 🎯 Kafka Event Flow

```
1. User สร้าง Order
   ↓
2. OrderService สร้าง Order ใน memory
   ↓
3. KafkaProducer ส่ง OrderCreatedEvent ไป topic "order-created"
   ↓
4. KafkaConsumerService รับ event และประมวลผล:
   - อัปเดต Stock
   - ส่งการแจ้งเตือนลูกค้า
   - บันทึกข้อมูล Analytics
```

## 📊 Mock Data

ระบบมี mock data สำหรับสินค้า:
- **PROD001**: Laptop Dell XPS 13 (฿45,000)
- **PROD002**: iPhone 15 Pro (฿35,000)
- **PROD003**: Samsung Galaxy S24 (฿28,000)
- **PROD004**: MacBook Air M2 (฿42,000)
- **PROD005**: AirPods Pro (฿8,500)

## 🔧 Configuration

### Kafka Settings (appsettings.Development.json)
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "ecommerce-order-group"
  }
}
```

## 🐳 Docker Services

### Services ที่รันใน Docker:
- **Zookeeper**: localhost:2181 - Kafka coordination service
- **Kafka**: localhost:9092 - Message broker
- **Kafka UI**: http://localhost:8085 - Web interface สำหรับจัดการ Kafka

### Kafka Topics ที่สร้างอัตโนมัติ:
- `order-created` (3 partitions)
- `stock-updated` (3 partitions) 
- `notification-sent` (3 partitions)

### Docker Commands:
```bash
# ดู logs ของ services
docker-compose logs -f

# ดู status ของ services
docker-compose ps

# รีสตาร์ท service เฉพาะ
docker-compose restart kafka

# ลบข้อมูลทั้งหมด (volumes)
docker-compose down -v
```

## 📈 Monitoring

### ตรวจสอบผ่าน Kafka UI:
- เข้าไปที่ http://localhost:8085
- ดู Topics, Messages, Consumers
- ตรวจสอบ Message flow แบบ real-time

### ตรวจสอบผ่าน Application Logs:
- Consumer service จะแสดง log เมื่อรับและประมวลผล events
- Producer service จะแสดง log เมื่อส่ง events สำเร็จ

## 🎓 Learning Objectives

โปรเจกต์นี้ช่วยให้เรียนรู้:
1. **Kafka Integration**: การใช้งาน Producer และ Consumer
2. **Event-Driven Architecture**: การออกแบบระบบแบบ event-driven
3. **Clean Architecture**: การแยกชั้น business logic
4. **MediatR Pattern**: การใช้งาน CQRS pattern
5. **Microservices Communication**: การสื่อสารระหว่าง services ผ่าน messaging

## 🚀 Next Steps

สามารถขยายต่อยอดได้:
1. เพิ่ม Database (Entity Framework)
2. เพิ่ม Authentication & Authorization
3. เพิ่ม Unit Tests
4. เพิ่ม Docker containerization
5. เพิ่ม Monitoring & Logging (ELK Stack)
6. เพิ่ม API Gateway
7. เพิ่ม Circuit Breaker pattern
