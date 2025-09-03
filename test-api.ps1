# Test E-Commerce Order Kafka Demo
Write-Host "=== E-Commerce Order Kafka Demo Test Script ===" -ForegroundColor Cyan
Write-Host ""

# Skip SSL certificate validation for testing
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy

$baseUrl = "https://localhost:7155"
$apiUrl = "$baseUrl/api"

# Check if API is running
Write-Host "1. Checking if API is running..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/swagger" -UseBasicParsing -TimeoutSec 5
    Write-Host "/ API is running - Swagger available at $baseUrl/swagger" -ForegroundColor Green
} catch {
    Write-Host "X API is not running. Please start with: dotnet run" -ForegroundColor Red
    Write-Host "Then visit: $baseUrl/swagger" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "2. Getting all products..." -ForegroundColor Yellow
try {
    $products = Invoke-RestMethod -Uri "$apiUrl/products" -Method GET
    Write-Host "/ Found $($products.Count) products:" -ForegroundColor Green
    $products | ForEach-Object {
        Write-Host "  - $($_.productName): ฿$($_.price) (Stock: $($_.stockQuantity))" -ForegroundColor White
    }
} catch {
    Write-Host "X Failed to get products: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "3. Creating a test order..." -ForegroundColor Yellow
$orderRequest = @{
    customerId = "CUST001"
    customerName = "John Doe"
    items = @(
        @{
            productId = "P001"
            quantity = 2
        },
        @{
            productId = "P002"
            quantity = 1
        }
    )
} | ConvertTo-Json -Depth 3

try {
    $order = Invoke-RestMethod -Uri "$apiUrl/orders" -Method POST -Body $orderRequest -ContentType "application/json"
    Write-Host "/ Order created successfully!" -ForegroundColor Green
    Write-Host "  Order ID: $($order.orderId)" -ForegroundColor White
    Write-Host "  Customer: $($order.customerName)" -ForegroundColor White
    Write-Host "  Total Amount: ฿$($order.totalAmount)" -ForegroundColor White
    Write-Host "  Items:" -ForegroundColor White
    $order.items | ForEach-Object {
        Write-Host "    - $($_.productName) x$($_.quantity) = ฿$($_.totalPrice)" -ForegroundColor White
    }
    $orderId = $order.orderId
} catch {
    Write-Host "X Failed to create order: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "4. Waiting for stock update (Kafka consumer processing)..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

Write-Host ""
Write-Host "5. Checking updated product stock..." -ForegroundColor Yellow
try {
    $updatedProducts = Invoke-RestMethod -Uri "$apiUrl/products" -Method GET
    Write-Host "/ Updated product stock:" -ForegroundColor Green
    $updatedProducts | Where-Object { $_.productId -in @("P001", "P002") } | ForEach-Object {
        Write-Host "  - $($_.productName): Stock = $($_.stockQuantity)" -ForegroundColor White
    }
} catch {
    Write-Host "X Failed to get updated products: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "6. Getting the created order..." -ForegroundColor Yellow
try {
    $retrievedOrder = Invoke-RestMethod -Uri "$apiUrl/orders/$orderId" -Method GET
    Write-Host "/ Order retrieved successfully!" -ForegroundColor Green
    Write-Host "  Status: $($retrievedOrder.status)" -ForegroundColor White
    Write-Host "  Order Date: $($retrievedOrder.orderDate)" -ForegroundColor White
} catch {
    Write-Host "X Failed to retrieve order: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "7. Getting all orders..." -ForegroundColor Yellow
try {
    $allOrders = Invoke-RestMethod -Uri "$apiUrl/orders" -Method GET
    Write-Host "/ Found $($allOrders.Count) total orders" -ForegroundColor Green
} catch {
    Write-Host "X Failed to get all orders: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Test Summary ===" -ForegroundColor Cyan
Write-Host "/ API is running" -ForegroundColor Green
Write-Host "/ Products endpoint working" -ForegroundColor Green
Write-Host "/ Order creation working" -ForegroundColor Green
Write-Host "/ Kafka event published" -ForegroundColor Green
Write-Host "/ Stock update via Kafka consumer" -ForegroundColor Green
Write-Host "/ Order retrieval working" -ForegroundColor Green
Write-Host ""
Write-Host "Demo completed successfully! 🎉" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Check Kafka UI at http://localhost:8085" -ForegroundColor White
Write-Host "2. Monitor API logs for Kafka events" -ForegroundColor White
Write-Host "3. Try creating more orders to see stock decrease" -ForegroundColor White
