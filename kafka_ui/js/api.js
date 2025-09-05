// API Service for E-commerce Kafka Monitor
class ApiService {
    constructor() {
        this.baseUrl = 'https://localhost:7155';
        this.timeout = 10000; // 10 seconds
    }

    // Generic fetch method with error handling
    async fetchWithTimeout(url, options = {}) {
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), this.timeout);

        try {
            const response = await fetch(url, {
                ...options,
                signal: controller.signal,
                headers: {
                    'Content-Type': 'application/json',
                    ...options.headers
                }
            });

            clearTimeout(timeoutId);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            clearTimeout(timeoutId);
            if (error.name === 'AbortError') {
                throw new Error('Request timeout');
            }
            throw error;
        }
    }

    // Orders API
    async createOrder(orderData) {
        try {
            console.log('Creating order:', orderData);
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Order/CreateOrder`, {
                method: 'POST',
                body: JSON.stringify(orderData)
            });
            console.log('Order created successfully:', response);
            return response;
        } catch (error) {
            console.error('Error creating order:', error);
            throw error;
        }
    }

    async getAllOrders() {
        try {
            console.log('Fetching all orders...');
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Order/GetAllOrders`);
            console.log('Orders fetched:', response);
            return response;
        } catch (error) {
            console.error('Error fetching orders:', error);
            throw error;
        }
    }

    async getOrderById(orderId) {
        try {
            console.log(`Fetching order ${orderId}...`);
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Order/GetOrder/${orderId}`);
            console.log('Order fetched:', response);
            return response;
        } catch (error) {
            console.error(`Error fetching order ${orderId}:`, error);
            throw error;
        }
    }

    async getOrdersByCustomer(customerId) {
        try {
            console.log(`Fetching orders for customer ${customerId}...`);
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Order/GetOrdersByCustomer/${customerId}`);
            console.log('Customer orders fetched:', response);
            return response;
        } catch (error) {
            console.error(`Error fetching orders for customer ${customerId}:`, error);
            throw error;
        }
    }

    // Products API
    async getProducts() {
        try {
            console.log('Fetching products...');
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Product/GetProducts`);
            console.log('Products fetched:', response);
            return response;
        } catch (error) {
            console.error('Error fetching products:', error);
            // Return mock data if API fails
            return this.getMockProducts();
        }
    }

    async getProductById(productId) {
        try {
            console.log(`Fetching product ${productId}...`);
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Product/GetProduct/${productId}`);
            console.log('Product fetched:', response);
            return response;
        } catch (error) {
            console.error(`Error fetching product ${productId}:`, error);
            throw error;
        }
    }

    async createProduct(productData) {
        try {
            console.log('Creating product:', productData);
            const payload = { request: productData };
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Product/CreateProduct`, {
                method: 'POST',
                body: JSON.stringify(payload)
            });
            console.log('Product created successfully:', response);
            return response;
        } catch (error) {
            console.error('Error creating product:', error);
            throw error;
        }
    }

    async updateProduct(productId, productData) {
        try {
            console.log(`Updating product ${productId}:`, productData);
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Product/UpdateProduct/${productId}`, {
                method: 'PUT',
                body: JSON.stringify(productData)
            });
            console.log('Product updated successfully:', response);
            return response;
        } catch (error) {
            console.error(`Error updating product ${productId}:`, error);
            throw error;
        }
    }

    async updateStock(productId, stockData) {
        try {
            console.log(`Updating stock for product ${productId}:`, stockData);
            console.log(`Updating stock for product ${productId}:`, stockData);
            const payload = { request: stockData };
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Product/UpdateStock`, {
                method: 'PUT',
                body: JSON.stringify(payload)
            });
            console.log('Stock updated successfully:', response);
            return response;
        } catch (error) {
            console.error(`Error updating stock for product ${productId}:`, error);
            throw error;
        }
    }

    async deleteProduct(productId) {
        try {
            console.log(`Deleting product ${productId}...`);
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Product/DeleteProduct/${productId}`, {
                method: 'DELETE'
            });
            console.log('Product deleted successfully:', response);
            return response;
        } catch (error) {
            console.error(`Error deleting product ${productId}:`, error);
            throw error;
        }
    }

    async getCategories() {
        try {
            console.log('Fetching categories...');
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Product/GetCategories`);
            console.log('Categories fetched:', response);
            return response;
        } catch (error) {
            console.error('Error fetching categories:', error);
            // Return default categories if API fails
            return {
                success: true,
                responseObject: ['Electronics', 'Clothing', 'Books', 'Home & Garden']
            };
        }
    }

    // Mock data for products (fallback)
    getMockProducts() {
        return {
            success: true,
            responseObject: [
                {
                    id: "PROD001",
                    name: "Laptop Dell XPS 13",
                    price: 45000.00,
                    stock: 10,
                    category: "Electronics",
                    imageUrl: "https://via.placeholder.com/300x200?text=Laptop+Dell+XPS+13",
                    description: "High-performance laptop with Intel Core i7"
                },
                {
                    id: "PROD002",
                    name: "iPhone 15 Pro",
                    price: 35000.00,
                    stock: 15,
                    category: "Electronics",
                    imageUrl: "https://via.placeholder.com/300x200?text=iPhone+15+Pro",
                    description: "Latest iPhone model with advanced features"
                },
                {
                    id: "PROD003",
                    name: "Samsung Galaxy S24",
                    price: 28000.00,
                    stock: 20,
                    category: "Electronics",
                    imageUrl: "https://via.placeholder.com/300x200?text=Samsung+Galaxy+S24",
                    description: "Android flagship phone with excellent camera"
                },
                {
                    id: "PROD004",
                    name: "MacBook Air M2",
                    price: 42000.00,
                    stock: 8,
                    category: "Electronics",
                    imageUrl: "https://via.placeholder.com/300x200?text=MacBook+Air+M2",
                    description: "Apple laptop with M2 chip and long battery life"
                },
                {
                    id: "PROD005",
                    name: "AirPods Pro",
                    price: 8500.00,
                    stock: 25,
                    category: "Electronics",
                    imageUrl: "https://via.placeholder.com/300x200?text=AirPods+Pro",
                    description: "Wireless earbuds with noise cancellation"
                }
            ]
        };
    }

    // Kafka Events API
    async getKafkaEvents() {
        try {
            console.log('Fetching Kafka events...');
            const response = await this.fetchWithTimeout(`${this.baseUrl}/KafkaEvents/GetRecentEvents`);
            console.log('Kafka events fetched:', response);
            return response;
        } catch (error) {
            console.error('Error fetching Kafka events:', error);
            throw error;
        }
    }

    async getKafkaEventStats() {
        try {
            console.log('Fetching Kafka event statistics...');
            const response = await this.fetchWithTimeout(`${this.baseUrl}/KafkaEvents/GetEventStats`);
            console.log('Kafka event stats fetched:', response);
            return response;
        } catch (error) {
            console.error('Error fetching Kafka event stats:', error);
            throw error;
        }
    }

    async clearKafkaEventLogs() {
        try {
            console.log('Clearing Kafka event logs...');
            const response = await this.fetchWithTimeout(`${this.baseUrl}/KafkaEvents/ClearEventLogs`, {
                method: 'POST'
            });
            console.log('Kafka event logs cleared:', response);
            return response;
        } catch (error) {
            console.error('Error clearing Kafka event logs:', error);
            throw error;
        }
    }

    // Health check
    async checkApiHealth() {
        try {
            const response = await this.fetchWithTimeout(`${this.baseUrl}/Health`);
            return { status: 'connected', data: response };
        } catch (error) {
            console.warn('API health check failed:', error);
            return { status: 'disconnected', error: error.message };
        }
    }

    // Utility methods
    formatCurrency(amount) {
        return new Intl.NumberFormat('th-TH', {
            style: 'currency',
            currency: 'THB'
        }).format(amount);
    }

    formatDate(dateString) {
        const date = new Date(dateString);
        return new Intl.DateTimeFormat('th-TH', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        }).format(date);
    }

    getStatusBadgeClass(status) {
        const statusMap = {
            'Pending': 'status-pending',
            'Processing': 'status-processing',
            'Completed': 'status-completed',
            'Cancelled': 'status-cancelled'
        };
        return statusMap[status] || 'badge-secondary';
    }

    getStockStatusClass(stock) {
        if (stock <= 5) return 'stock-low';
        if (stock <= 15) return 'stock-medium';
        return 'stock-high';
    }

    // Error handling utilities
    handleApiError(error) {
        let message = 'เกิดข้อผิดพลาดในการเชื่อมต่อ API';
        
        if (error.message === 'Request timeout') {
            message = 'การเชื่อมต่อหมดเวลา กรุณาลองใหม่อีกครั้ง';
        } else if (error.message.includes('HTTP error')) {
            const status = error.message.match(/status: (\d+)/)?.[1];
            switch (status) {
                case '400':
                    message = 'ข้อมูลที่ส่งไม่ถูกต้อง';
                    break;
                case '404':
                    message = 'ไม่พบข้อมูลที่ต้องการ';
                    break;
                case '500':
                    message = 'เกิดข้อผิดพลาดในเซิร์ฟเวอร์';
                    break;
                default:
                    message = `เกิดข้อผิดพลาด (${status})`;
            }
        } else if (error.message.includes('Failed to fetch')) {
            message = 'ไม่สามารถเชื่อมต่อกับเซิร์ฟเวอร์ได้ กรุณาตรวจสอบว่า API Server ทำงานอยู่';
        }

        return message;
    }

    // Show notification
    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        document.body.appendChild(notification);

        // Auto remove after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 5000);
    }

    // Loading state management
    showLoading(element) {
        if (element) {
            element.disabled = true;
            const originalText = element.textContent;
            element.innerHTML = '<span class="loading-spinner me-2"></span>Loading...';
            element.dataset.originalText = originalText;
        }
    }

    hideLoading(element) {
        if (element && element.dataset.originalText) {
            element.disabled = false;
            element.textContent = element.dataset.originalText;
            delete element.dataset.originalText;
        }
    }

    // Connection status management
    updateConnectionStatus(status) {
        const statusElement = document.getElementById('connection-status');
        const statusIcon = statusElement?.previousElementSibling;
        
        if (statusElement && statusIcon) {
            statusElement.textContent = status === 'connected' ? 'Connected' : 'Disconnected';
            statusIcon.className = `fas fa-circle me-1 ${status === 'connected' ? 'text-success' : 'text-danger'}`;
        }
    }

    // Periodic health check
    startHealthCheck() {
        this.checkApiHealth().then(result => {
            this.updateConnectionStatus(result.status);
        });

        // Check every 30 seconds
        setInterval(async () => {
            const result = await this.checkApiHealth();
            this.updateConnectionStatus(result.status);
        }, 30000);
    }
}

// Create global API service instance
window.apiService = new ApiService();

// Start health check when page loads
document.addEventListener('DOMContentLoaded', () => {
    window.apiService.startHealthCheck();
});
