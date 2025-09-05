# E-Commerce Kafka Frontend UI

Frontend application สำหรับทดสอบระบบ E-commerce Order Processing ที่ใช้ Apache Kafka สำหรับ event-driven architecture

## 📋 ข้อกำหนดระบบ

- Web Browser (Chrome, Firefox, Safari, Edge)
- .NET 6 SDK
- Apache Kafka
- Backend API (TQM.BackOffice.API)

## 🚀 การติดตั้งและเริ่มต้นใช้งาน

### 1. เริ่มต้น Kafka

```bash
# ใช้ Docker Compose (แนะนำ)
docker-compose up -d

# หรือใช้ script ที่เตรียมไว้
scripts/start-kafka.bat
```

### 2. เริ่มต้น Backend API

```bash
# ไปที่โฟลเดอร์ API
cd src/API

# รัน API
dotnet run

# หรือใช้ script ที่เตรียมไว้
scripts/run-project.bat
```

API จะทำงานที่: `https://localhost:7155`

### 3. เปิด Frontend UI

เปิดไฟล์ `kafka_ui/index.html` ในเว็บเบราว์เซอร์

```bash
# Windows
start kafka_ui/index.html

# macOS
open kafka_ui/index.html

# Linux
xdg-open kafka_ui/index.html
```

## 🎯 ฟีเจอร์หลัก

### 📊 Dashboard
- แสดงสถิติการสั่งซื้อแบบ real-time
- กราฟแสดงยอดขายรายวัน
- กราฟแสดงจำนวนออเดอร์

### 🛒 Orders Management
- สร้างออเดอร์ใหม่
- ดูรายการออเดอร์ทั้งหมด
- ค้นหาออเดอร์ตามลูกค้า
- ดูรายละเอียดออเดอร์

### 📦 Products Catalog
- แสดงรายการสินค้า
- ข้อมูล mock สำหรับการทดสอบ

### ⚡ Kafka Monitoring
- ติดตาม Kafka events แบบ real-time
- แสดงจำนวน events ในแต่ละ topic:
  - `order-created`: เมื่อมีการสร้างออเดอร์
  - `stock-updated`: เมื่อมีการอัปเดตสต็อก
  - `notification-sent`: เมื่อมีการส่งการแจ้งเตือน

### 📝 Event Logs
- บันทึก events ทั้งหมดพร้อม timestamp
- กรองตาม event type
- Export logs สำหรับการ debug

## 🔧 การใช้งาน

### สร้างออเดอร์ใหม่

1. ไปที่แท็บ "Orders"
2. คลิก "สร้างออเดอร์ใหม่"
3. กรอกข้อมูล:
   - ชื่อลูกค้า
   - อีเมล
   - เลือกสินค้าและจำนวน
4. คลิก "สร้างออเดอร์"

### ติดตาม Kafka Events

1. ไปที่แท็บ "Kafka Monitor"
2. สังเกตการเปลี่ยนแปลงของ event counters
3. ดู event logs ในแท็บ "Event Logs"

### ดูสถิติ

1. ไปที่แท็บ "Dashboard"
2. ดูกราฟและสถิติที่อัปเดตแบบ real-time
3. ใช้ปุ่ม "รีเฟรช" เพื่ออัปเดตข้อมูล

## 🎨 ธีม

- **Light Theme**: ธีมสว่าง (default)
- **Dark Theme**: ธีมมืด

สลับธีมได้ที่ปุ่มด้านขวาบนของหน้าจอ

## 🔍 การทดสอบ

### ทดสอบการเชื่อมต่อ API

1. เปิด Developer Tools (F12)
2. ดูใน Console สำหรับ connection status
3. หากเชื่อมต่อสำเร็จจะแสดง "✅ API Connected"
4. หากเชื่อมต่อไม่สำเร็จจะแสดง "❌ API Disconnected"

### ทดสอบ Kafka Events

1. สร้างออเดอร์ใหม่
2. ตรวจสอบ Kafka Monitor ว่ามี events เพิ่มขึ้น
3. ดู Event Logs ว่ามีการบันทึก events

### ทดสอบ Mock Data

หาก API ไม่พร้อมใช้งาน ระบบจะใช้ mock data:
- สินค้า: แสดงสินค้าตัวอย่าง 6 รายการ
- ออเดอร์: แสดงข้อความแจ้งเตือนการเชื่อมต่อ

## 🐛 การแก้ไขปัญหา

### API ไม่ตอบสนอง

1. ตรวจสอบว่า Backend API ทำงานอยู่ที่ `https://localhost:7155`
2. ตรวจสอบ CORS settings ใน API
3. ดู Network tab ใน Developer Tools
4. ตรวจสอบ SSL certificate หากใช้ HTTPS

### Kafka Events ไม่แสดง

1. ตรวจสอบว่า Kafka ทำงานอยู่
2. ตรวจสอบ Kafka Consumer ใน Backend
3. ลองสร้างออเดอร์ใหม่เพื่อ trigger events

### หน้าเว็บไม่แสดงผลถูกต้อง

1. ล้าง browser cache
2. ตรวจสอบ Console errors
3. ตรวจสอบว่าไฟล์ CSS และ JS โหลดสำเร็จ

## 📁 โครงสร้างไฟล์

```
kafka_ui/
├── index.html          # หน้าหลักของ application
├── css/
│   └── style.css       # Styling และ themes
├── js/
│   ├── api.js          # API service layer
│   ├── kafka-monitor.js # Kafka monitoring และ simulation
│   └── app.js          # Main application controller
└── README.md           # เอกสารนี้
```

## 🔗 API Endpoints

### Orders API
- `POST /Order/CreateOrder` - สร้างออเดอร์ใหม่
- `GET /Order/GetAllOrders` - ดูออเดอร์ทั้งหมด
- `GET /Order/GetOrder/{orderId}` - ดูออเดอร์ตาม ID
- `GET /Order/GetOrdersByCustomer/{customerId}` - ดูออเดอร์ตามลูกค้า

### Products API
- `GET /Product/GetProducts` - ดูสินค้าทั้งหมด
- `GET /Product/GetProduct/{productId}` - ดูสินค้าตาม ID

### Health Check API
- `GET /Health` - ตรวจสอบสถานะ API
- `GET /Health/ping` - Ping test
- `GET /Health/version` - ดูเวอร์ชัน API

## 📊 Kafka Topics

- `order-created` - เมื่อมีการสร้างออเดอร์ใหม่
- `stock-updated` - เมื่อมีการอัปเดตสต็อกสินค้า
- `notification-sent` - เมื่อมีการส่งการแจ้งเตือน

## 🎓 การเรียนรู้

Frontend นี้ออกแบบมาเพื่อการเรียนรู้:

1. **Event-Driven Architecture**: ดูการทำงานของ Kafka events
2. **Real-time Updates**: สังเกตการอัปเดตข้อมูลแบบ real-time
3. **API Integration**: เรียนรู้การเชื่อมต่อ Frontend กับ Backend
4. **Responsive Design**: ทดสอบบนอุปกรณ์ต่างๆ

## 📞 การสนับสนุน

หากพบปัญหาหรือต้องการความช่วยเหลือ:

1. ตรวจสอบ Console logs ใน Developer Tools
2. ดู Event Logs ใน application
3. ตรวจสอบ Backend API logs
4. ตรวจสอบ Kafka logs

---

**หมายเหตุ**: Frontend นี้ใช้ Vanilla JavaScript และ Bootstrap 5 เพื่อความง่ายในการทดสอบและไม่ต้องติดตั้ง dependencies เพิ่มเติม
