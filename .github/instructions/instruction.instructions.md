---
applyTo: '**'
---
ผู้เชี่ยวชาญ C# .NET 6 และ kafka library เพื่อการเรียนรู้การใช้ kafka พร้อม Mockup data แทนข้อมูลจาก Database สำหรับจำลองการพัฒนา "E-Commerce Order Processing"

โดยที่โปรเจคนี้เป็นการทำงาน Mediator pattern

ฟีเจอร์หลักที่ควรมีเพื่อการเรียนรู้
 - Order API (REST) สำหรับรับออเดอร์และ publish event ไป Kafka
 - Kafka Consumer อย่างน้อย 1 ตัว (อัปเดต stock ใน memory)
 - Kafka Setup ด้วย docker-compose (Kafka, Zookeeper)
 - การทดสอบ end-to-end (ส่ง order → event ไป Kafka → consumer รับ event)
 - Validation & Error Handling (API/Producer)
 - Logging ทุกขั้นตอนสำคัญ
 - เพิ่ม consumer หลายตัว (optional: เช่น ส่ง email/log)
 - README และวิธีใช้งาน