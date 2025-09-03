# E-Commerce Order Kafka Demo

ระบบ E-Commerce Order Processing ที่ใช้ Kafka สำหรับ Event-Driven Architecture

## Architecture Overview

```
API (Orders Controller) 
    ↓ (POST /orders)
Order Service (MediatR) 
    ↓ (Publish Event)
Kafka Topic: "orders"
    ↓ (Consume Event)
Stock Update Consumer
    ↓ (Update Stock)
In-Memory Product Repository
```

## Features

- **Order Management**: สร้างและดูข้อมูลออเดอร์
- **Product Management**: ดูข้อมูลสินค้าและ stock
- **Event-Driven Stock Update**: อัปเดต stock อัตโนมัติผ่าน Kafka
- **In-Memory Storage**: ใช้ In-Memory สำหรับ demo (แทน database)

## Quick Start

### 1. เริ่มต้น Kafka และ Zookeeper

```bash
# ใน root directory ของโปรเจค
docker-compose up -d
```

Services ที่จะเริ่มทำงาน:
- **Zookeeper**: `localhost:2181`
- **Kafka**: `localhost:9092`
- **Kafka UI**: `http://localhost:8080` (สำหรับดู Kafka topics และ messages)

### 2. Build และ Run API

```bash
cd src/API
dotnet build
dotnet run
```

API จะทำงานที่: `https://localhost:7000` หรือ `http://localhost:5000`

### 3. Test API

#### ดู Products ที่มี (มี initial data 5 products)
```bash
GET /api/products
```

#### สร้าง Order
```bash
POST /api/orders
Content-Type: application/json

{
  "customerId": "C001",
  "customerName": "John Doe",
  "items": [
    {
      "productId": "P001",
      "quantity": 2
    },
    {
      "productId": "P002", 
      "quantity": 1
    }
  ]
}
```

#### ดู Orders ที่สร้างแล้ว
```bash
GET /api/orders
```

#### ดู Order เฉพาะ
```bash
GET /api/orders/{orderId}
```

## API Endpoints

### Products
- `GET /api/products` - ดูสินค้าทั้งหมด
- `GET /api/products/{productId}` - ดูสินค้าเฉพาะ

### Orders  
- `POST /api/orders` - สร้างออเดอร์ใหม่
- `GET /api/orders` - ดูออเดอร์ทั้งหมด
- `GET /api/orders/{orderId}` - ดูออเดอร์เฉพาะ

## Initial Data

### Products
- P001: iPhone 15 (฿30,000, Stock: 50)
- P002: Samsung Galaxy S24 (฿25,000, Stock: 30)
- P003: MacBook Pro (฿80,000, Stock: 20)
- P004: iPad Air (฿20,000, Stock: 40)
- P005: AirPods Pro (฿8,000, Stock: 100)

## How It Works

1. **เมื่อสร้าง Order**:
   - API รับ request และ validate products
   - คำนวณราคารวม
   - สร้าง Order และเก็บใน memory
   - Publish `OrderCreatedEvent` ไป Kafka topic "orders"

2. **Stock Update Consumer**:
   - Subscribe to "orders" topic
   - เมื่อได้รับ event จะอัปเดต stock ของสินค้าที่สั่งซื้อ
   - ลด stock quantity ตามจำนวนที่สั่ง

3. **Event Flow**:
   ```
   Order Created → Kafka Event → Stock Updated
   ```

## Monitoring

### Kafka UI
- URL: `http://localhost:8080`
- ดู Topics, Messages, Consumers
- Monitor Event Flow

### Logs
- API logs จะแสดงการ publish events
- Consumer logs จะแสดงการ process events และ update stock

## Swagger Documentation

เมื่อ run API จะมี Swagger UI ที่: `https://localhost:7000/swagger`

## Architecture Benefits

- **Decoupling**: Order service ไม่ต้องรู้จัก Stock service โดยตรง
- **Scalability**: Consumer สามารถ scale แยกได้
- **Reliability**: Kafka รับประกันการส่ง message
- **Monitoring**: สามารถติดตาม event flow ได้ผ่าน Kafka UI

## Development Notes

- ใช้ In-Memory storage สำหรับ demo (ข้อมูลจะหายเมื่อ restart)
- Consumer ทำงานเป็น Background Service
- ใช้ MediatR pattern สำหรับ CQRS
- Clean Architecture ตาม folder structure

## Next Steps

สำหรับ Production:
- เปลี่ยนจาก In-Memory เป็น Database
- เพิ่ม Error Handling และ Retry Logic
- เพิ่ม Authentication/Authorization
- เพิ่ม Health Checks
- เพิ่ม Monitoring และ Metrics
