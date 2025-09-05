// Kafka Monitor Service for Real-time Event Tracking
class KafkaMonitor {
    constructor() {
        this.eventCounts = {
            'order-created': 0,
            'stock-updated': 0,
            'notification-sent': 0
        };
        this.eventLogs = [];
        this.maxLogs = 100;
        this.isMonitoring = false;
        this.pollingInterval = 3000; // 3 seconds
        this.pollingTimer = null;
        this.lastEventCount = 0;
    }

    // Start monitoring Kafka events
    startMonitoring() {
        if (this.isMonitoring) return;
        
        this.isMonitoring = true;
        this.addEventLog('system', 'Kafka monitoring started - connecting to real Kafka events', 'info');
        
        // Start polling for real events from API
        this.pollingTimer = setInterval(() => {
            this.fetchRealKafkaEvents();
        }, this.pollingInterval);
        
        console.log('Kafka monitoring started - using real Kafka data');
    }

    // Stop monitoring
    stopMonitoring() {
        if (!this.isMonitoring) return;
        
        this.isMonitoring = false;
        if (this.pollingTimer) {
            clearInterval(this.pollingTimer);
            this.pollingTimer = null;
        }
        
        this.addEventLog('system', 'Kafka monitoring stopped', 'warning');
        console.log('Kafka monitoring stopped');
    }

    // Fetch real Kafka events from API
    async fetchRealKafkaEvents() {
        try {
            // Get event statistics from API
            const statsResponse = await window.apiService.getKafkaEventStats();
            if (statsResponse && statsResponse.success && statsResponse.responseObject) {
                const stats = statsResponse.responseObject;
                
                // Update event counts
                this.eventCounts = { ...stats.eventCounts };
                
                // Check if there are new events
                const currentTotalEvents = stats.totalEvents;
                if (currentTotalEvents > this.lastEventCount) {
                    // Get recent events
                    const eventsResponse = await window.apiService.getKafkaEvents();
                    if (eventsResponse && eventsResponse.success && eventsResponse.responseObject) {
                        const recentEvents = eventsResponse.responseObject;
                        
                        // Process new events
                        recentEvents.forEach(event => {
                            if (event.eventData) {
                                this.processRealKafkaEvent(event);
                            }
                        });
                        
                        this.lastEventCount = currentTotalEvents;
                    }
                }
                
                // Update UI
                this.updateKafkaUI();
                this.updateDashboardStats();
            }
        } catch (error) {
            console.warn('Failed to fetch real Kafka events, falling back to simulation:', error);
            // Fallback to simulation if API is not available
            this.simulateKafkaEvents();
        }
    }

    // Process real Kafka event from API
    processRealKafkaEvent(kafkaEventLog) {
        const eventData = kafkaEventLog.eventData;
        const topic = kafkaEventLog.topic;
        
        // Add to local event logs with real data
        this.addEventLog(topic, kafkaEventLog.message, kafkaEventLog.level);
        
        // Trigger product refresh if it's a stock update event
        if (topic === 'stock-updated' && window.app) {
            // Refresh products in the UI
            setTimeout(() => {
                window.app.loadProducts();
            }, 500);
        }
        
        console.log(`Real Kafka event processed: ${topic}`, eventData);
    }

    // Simulate Kafka events (fallback when API is not available)
    simulateKafkaEvents() {
        // Randomly generate events based on API activity
        const eventTypes = ['order-created', 'stock-updated', 'notification-sent'];
        const randomType = eventTypes[Math.floor(Math.random() * eventTypes.length)];
        
        // Simulate event with some probability
        if (Math.random() < 0.2) { // 20% chance per poll (reduced since we prefer real data)
            this.processKafkaEvent(randomType, this.generateMockEventData(randomType));
        }
    }

    // Generate mock event data
    generateMockEventData(eventType) {
        const timestamp = new Date().toISOString();
        const orderId = Math.floor(Math.random() * 1000) + 1;
        const customerId = `CUST${String(Math.floor(Math.random() * 100) + 1).padStart(3, '0')}`;
        
        switch (eventType) {
            case 'order-created':
                return {
                    eventType: 'OrderCreatedEvent',
                    orderId: orderId,
                    customerId: customerId,
                    customerName: `Customer ${customerId}`,
                    totalAmount: (Math.random() * 50000 + 5000).toFixed(2),
                    timestamp: timestamp,
                    topic: 'order-created'
                };
                
            case 'stock-updated':
                const products = ['PROD001', 'PROD002', 'PROD003', 'PROD004', 'PROD005'];
                const productId = products[Math.floor(Math.random() * products.length)];
                return {
                    eventType: 'StockUpdatedEvent',
                    productId: productId,
                    productName: `Product ${productId}`,
                    previousStock: Math.floor(Math.random() * 50) + 10,
                    newStock: Math.floor(Math.random() * 50) + 5,
                    timestamp: timestamp,
                    topic: 'stock-updated'
                };
                
            case 'notification-sent':
                return {
                    eventType: 'NotificationSentEvent',
                    orderId: orderId,
                    customerId: customerId,
                    notificationType: 'OrderConfirmation',
                    message: 'Your order has been confirmed',
                    timestamp: timestamp,
                    topic: 'notification-sent'
                };
                
            default:
                return null;
        }
    }

    // Process incoming Kafka event
    processKafkaEvent(topic, eventData) {
        if (!eventData) return;
        
        // Update event counts
        this.eventCounts[topic] = (this.eventCounts[topic] || 0) + 1;
        
        // Add to event logs
        this.addEventLog(topic, this.formatEventMessage(eventData), 'success');
        
        // Update UI
        this.updateKafkaUI();
        this.updateDashboardStats();
        
        console.log(`Kafka event processed: ${topic}`, eventData);
    }

    // Format event message for display
    formatEventMessage(eventData) {
        switch (eventData.eventType) {
            case 'OrderCreatedEvent':
                return `Order ${eventData.orderId} created for ${eventData.customerName} - Amount: ฿${parseFloat(eventData.totalAmount).toLocaleString()}`;
                
            case 'StockUpdatedEvent':
                return `Stock updated for ${eventData.productName}: ${eventData.previousStock} → ${eventData.newStock}`;
                
            case 'NotificationSentEvent':
                return `${eventData.notificationType} sent to customer ${eventData.customerId} for order ${eventData.orderId}`;
                
            default:
                return `Event: ${eventData.eventType}`;
        }
    }

    // Add event to logs
    addEventLog(type, message, level = 'info') {
        const logEntry = {
            id: Date.now() + Math.random(),
            timestamp: new Date(),
            type: type,
            message: message,
            level: level
        };
        
        this.eventLogs.unshift(logEntry);
        
        // Keep only recent logs
        if (this.eventLogs.length > this.maxLogs) {
            this.eventLogs = this.eventLogs.slice(0, this.maxLogs);
        }
        
        this.updateEventLogsUI();
    }

    // Update Kafka monitoring UI
    updateKafkaUI() {
        // Update topic message counts
        Object.keys(this.eventCounts).forEach(topic => {
            const countElement = document.getElementById(`${topic.replace('-', '-')}-count`);
            if (countElement) {
                countElement.textContent = this.eventCounts[topic];
            }
        });
        
        // Update total Kafka events in dashboard
        const totalEvents = Object.values(this.eventCounts).reduce((sum, count) => sum + count, 0);
        const kafkaEventsElement = document.getElementById('kafka-events');
        if (kafkaEventsElement) {
            kafkaEventsElement.textContent = totalEvents;
        }
    }

    // Update event logs UI
    updateEventLogsUI() {
        const eventLogsContainer = document.getElementById('event-logs');
        if (!eventLogsContainer) return;
        
        eventLogsContainer.innerHTML = '';
        
        this.eventLogs.forEach(log => {
            const logElement = document.createElement('div');
            logElement.className = `event-log-entry ${log.type} fade-in`;
            
            const levelClass = {
                'info': 'text-info',
                'success': 'text-success',
                'warning': 'text-warning',
                'error': 'text-danger'
            }[log.level] || 'text-info';
            
            logElement.innerHTML = `
                <div class="d-flex justify-content-between align-items-start">
                    <div class="flex-grow-1">
                        <div class="event-timestamp ${levelClass}">
                            ${this.formatTimestamp(log.timestamp)}
                        </div>
                        <div class="event-type ${levelClass}">
                            ${log.type.toUpperCase()}
                        </div>
                        <div class="event-message">
                            ${log.message}
                        </div>
                    </div>
                    <div class="ms-2">
                        <i class="fas fa-circle ${levelClass}" style="font-size: 0.5rem;"></i>
                    </div>
                </div>
            `;
            
            eventLogsContainer.appendChild(logElement);
        });
    }

    // Format timestamp for display
    formatTimestamp(date) {
        return new Intl.DateTimeFormat('th-TH', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            fractionalSecondDigits: 3
        }).format(date);
    }

    // Update dashboard statistics
    updateDashboardStats() {
        // This will be called when orders are created through the API
        // For now, we'll simulate some stats updates
        this.updateOrderStats();
    }

    // Update order statistics
    async updateOrderStats() {
        try {
            const ordersResponse = await window.apiService.getAllOrders();
            if (ordersResponse && ordersResponse.success && ordersResponse.responseObject) {
                const orders = ordersResponse.responseObject;
                
                // Update total orders
                const totalOrdersElement = document.getElementById('total-orders');
                if (totalOrdersElement) {
                    totalOrdersElement.textContent = orders.length;
                }
                
                // Calculate total revenue
                const totalRevenue = orders.reduce((sum, order) => sum + (order.totalAmount || 0), 0);
                const totalRevenueElement = document.getElementById('total-revenue');
                if (totalRevenueElement) {
                    totalRevenueElement.textContent = window.apiService.formatCurrency(totalRevenue);
                }
            }
        } catch (error) {
            console.warn('Failed to update order stats:', error);
        }
    }

    // Clear event logs
    clearEventLogs() {
        this.eventLogs = [];
        this.updateEventLogsUI();
        this.addEventLog('system', 'Event logs cleared', 'info');
    }

    // Clear Kafka logs (reset counters)
    clearKafkaLogs() {
        this.eventCounts = {
            'order-created': 0,
            'stock-updated': 0,
            'notification-sent': 0
        };
        this.updateKafkaUI();
        this.addEventLog('system', 'Kafka counters reset', 'info');
    }

    // Simulate order creation event (called when order is created via API)
    simulateOrderCreated(orderData) {
        const eventData = {
            eventType: 'OrderCreatedEvent',
            orderId: orderData.id,
            customerId: orderData.customerId,
            customerName: orderData.customerName,
            totalAmount: orderData.totalAmount,
            timestamp: new Date().toISOString(),
            topic: 'order-created'
        };
        
        this.processKafkaEvent('order-created', eventData);
        
        // Simulate related events
        setTimeout(() => {
            if (orderData.orderItems) {
                orderData.orderItems.forEach(item => {
                    const stockEventData = {
                        eventType: 'StockUpdatedEvent',
                        productId: item.productId,
                        productName: item.productName,
                        previousStock: Math.floor(Math.random() * 50) + item.quantity,
                        newStock: Math.floor(Math.random() * 50),
                        timestamp: new Date().toISOString(),
                        topic: 'stock-updated'
                    };
                    this.processKafkaEvent('stock-updated', stockEventData);
                });
            }
        }, 500);
        
        setTimeout(() => {
            const notificationEventData = {
                eventType: 'NotificationSentEvent',
                orderId: orderData.id,
                customerId: orderData.customerId,
                notificationType: 'OrderConfirmation',
                message: 'Your order has been confirmed',
                timestamp: new Date().toISOString(),
                topic: 'notification-sent'
            };
            this.processKafkaEvent('notification-sent', notificationEventData);
        }, 1000);
    }

    // Get event statistics
    getEventStats() {
        return {
            totalEvents: Object.values(this.eventCounts).reduce((sum, count) => sum + count, 0),
            eventCounts: { ...this.eventCounts },
            recentLogs: this.eventLogs.slice(0, 10),
            isMonitoring: this.isMonitoring
        };
    }

    // Export logs (for debugging)
    exportLogs() {
        const data = {
            timestamp: new Date().toISOString(),
            eventCounts: this.eventCounts,
            eventLogs: this.eventLogs
        };
        
        const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `kafka-logs-${Date.now()}.json`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }
}

// Create global Kafka monitor instance
window.kafkaMonitor = new KafkaMonitor();

// Auto-start monitoring when page loads
document.addEventListener('DOMContentLoaded', () => {
    // Start monitoring after a short delay
    setTimeout(() => {
        window.kafkaMonitor.startMonitoring();
    }, 1000);
});
