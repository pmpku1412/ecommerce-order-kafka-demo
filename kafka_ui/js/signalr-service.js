// SignalR Service for Real-time Kafka Events
class SignalRService {
    constructor() {
        this.connection = null;
        this.isConnected = false;
        this.reconnectAttempts = 0;
        this.maxReconnectAttempts = 5;
        this.reconnectDelay = 3000; // 3 seconds
        this.hubUrl = 'https://localhost:7155/kafkaEventsHub';
        this.eventHandlers = new Map();
        this.connectionStateHandlers = [];
    }

    // Initialize and start SignalR connection
    async connect() {
        try {
            if (this.connection && this.isConnected) {
                console.log('SignalR already connected');
                return true;
            }

            console.log('Initializing SignalR connection...');
            
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(this.hubUrl, {
                    withCredentials: false,
                    transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
                })
                .withAutomaticReconnect({
                    nextRetryDelayInMilliseconds: retryContext => {
                        if (retryContext.previousRetryCount < 3) {
                            return 1000; // 1 second
                        } else if (retryContext.previousRetryCount < 6) {
                            return 3000; // 3 seconds
                        } else {
                            return 5000; // 5 seconds
                        }
                    }
                })
                .configureLogging(signalR.LogLevel.Information)
                .build();

            // Set up event handlers
            this.setupEventHandlers();
            this.setupConnectionHandlers();

            // Start connection
            await this.connection.start();
            this.isConnected = true;
            this.reconnectAttempts = 0;
            
            console.log('SignalR connected successfully');
            this.notifyConnectionState('connected');
            
            return true;
        } catch (error) {
            console.error('SignalR connection failed:', error);
            this.isConnected = false;
            this.notifyConnectionState('disconnected', error.message);
            
            // Attempt reconnection
            this.scheduleReconnect();
            return false;
        }
    }

    // Disconnect from SignalR
    async disconnect() {
        try {
            if (this.connection) {
                await this.connection.stop();
                this.isConnected = false;
                console.log('SignalR disconnected');
                this.notifyConnectionState('disconnected');
            }
        } catch (error) {
            console.error('Error disconnecting SignalR:', error);
        }
    }

    // Setup SignalR event handlers
    setupEventHandlers() {
        if (!this.connection) return;

        // Kafka event received
        this.connection.on('KafkaEventReceived', (data) => {
            console.log('Kafka event received via SignalR:', data);
            this.handleEvent('kafkaEvent', data);
        });

        // Event statistics updated
        this.connection.on('EventStatsUpdated', (data) => {
            console.log('Event stats updated via SignalR:', data);
            this.handleEvent('eventStats', data);
        });

        // Event log added
        this.connection.on('EventLogAdded', (data) => {
            console.log('Event log added via SignalR:', data);
            this.handleEvent('eventLog', data);
        });

        // Connection status changed
        this.connection.on('ConnectionStatusChanged', (data) => {
            console.log('Connection status changed via SignalR:', data);
            this.handleEvent('connectionStatus', data);
        });

        // Notification received
        this.connection.on('NotificationReceived', (data) => {
            console.log('Notification received via SignalR:', data);
            this.handleEvent('notification', data);
        });

        // Product data updated
        this.connection.on('ProductDataUpdated', (data) => {
            console.log('Product data updated via SignalR:', data);
            this.handleEvent('productDataUpdated', data);
        });

        // Order data updated
        this.connection.on('OrderDataUpdated', (data) => {
            console.log('Order data updated via SignalR:', data);
            this.handleEvent('orderDataUpdated', data);
        });

        // Hub-specific events
        this.connection.on('JoinedKafkaMonitoring', (data) => {
            console.log('Joined Kafka monitoring group:', data);
            this.handleEvent('joinedMonitoring', data);
        });

        this.connection.on('LeftKafkaMonitoring', (data) => {
            console.log('Left Kafka monitoring group:', data);
            this.handleEvent('leftMonitoring', data);
        });
    }

    // Setup connection state handlers
    setupConnectionHandlers() {
        if (!this.connection) return;

        this.connection.onclose((error) => {
            console.log('SignalR connection closed:', error);
            this.isConnected = false;
            this.notifyConnectionState('disconnected', error?.message);
            this.scheduleReconnect();
        });

        this.connection.onreconnecting((error) => {
            console.log('SignalR reconnecting:', error);
            this.isConnected = false;
            this.notifyConnectionState('reconnecting', error?.message);
        });

        this.connection.onreconnected((connectionId) => {
            console.log('SignalR reconnected:', connectionId);
            this.isConnected = true;
            this.reconnectAttempts = 0;
            this.notifyConnectionState('connected');
        });
    }

    // Handle incoming events
    handleEvent(eventType, data) {
        const handlers = this.eventHandlers.get(eventType);
        if (handlers) {
            handlers.forEach(handler => {
                try {
                    handler(data);
                } catch (error) {
                    console.error(`Error in event handler for ${eventType}:`, error);
                }
            });
        }
    }

    // Register event handler
    on(eventType, handler) {
        if (!this.eventHandlers.has(eventType)) {
            this.eventHandlers.set(eventType, []);
        }
        this.eventHandlers.get(eventType).push(handler);
    }

    // Unregister event handler
    off(eventType, handler) {
        const handlers = this.eventHandlers.get(eventType);
        if (handlers) {
            const index = handlers.indexOf(handler);
            if (index > -1) {
                handlers.splice(index, 1);
            }
        }
    }

    // Register connection state handler
    onConnectionStateChanged(handler) {
        this.connectionStateHandlers.push(handler);
    }

    // Notify connection state change
    notifyConnectionState(state, message = '') {
        this.connectionStateHandlers.forEach(handler => {
            try {
                handler(state, message);
            } catch (error) {
                console.error('Error in connection state handler:', error);
            }
        });
    }

    // Schedule reconnection attempt
    scheduleReconnect() {
        if (this.reconnectAttempts >= this.maxReconnectAttempts) {
            console.log('Max reconnection attempts reached');
            this.notifyConnectionState('failed', 'Max reconnection attempts reached');
            return;
        }

        this.reconnectAttempts++;
        const delay = this.reconnectDelay * this.reconnectAttempts;
        
        console.log(`Scheduling reconnection attempt ${this.reconnectAttempts} in ${delay}ms`);
        
        setTimeout(async () => {
            console.log(`Reconnection attempt ${this.reconnectAttempts}`);
            await this.connect();
        }, delay);
    }

    // Send message to hub (if needed for future features)
    async sendMessage(methodName, ...args) {
        if (!this.isConnected || !this.connection) {
            console.warn('SignalR not connected, cannot send message');
            return false;
        }

        try {
            await this.connection.invoke(methodName, ...args);
            return true;
        } catch (error) {
            console.error(`Error sending message ${methodName}:`, error);
            return false;
        }
    }

    // Get connection state
    getConnectionState() {
        if (!this.connection) return 'disconnected';
        
        switch (this.connection.state) {
            case signalR.HubConnectionState.Connected:
                return 'connected';
            case signalR.HubConnectionState.Connecting:
                return 'connecting';
            case signalR.HubConnectionState.Reconnecting:
                return 'reconnecting';
            case signalR.HubConnectionState.Disconnected:
                return 'disconnected';
            case signalR.HubConnectionState.Disconnecting:
                return 'disconnecting';
            default:
                return 'unknown';
        }
    }

    // Check if connected
    get connected() {
        return this.isConnected && this.connection?.state === signalR.HubConnectionState.Connected;
    }

    // Get connection info
    getConnectionInfo() {
        return {
            isConnected: this.isConnected,
            connectionId: this.connection?.connectionId,
            state: this.getConnectionState(),
            reconnectAttempts: this.reconnectAttempts,
            maxReconnectAttempts: this.maxReconnectAttempts
        };
    }

    // Convenience methods for specific events
    onProductDataUpdated(handler) {
        this.on('productDataUpdated', handler);
    }

    onOrderDataUpdated(handler) {
        this.on('orderDataUpdated', handler);
    }

    onKafkaEvent(handler) {
        this.on('kafkaEvent', handler);
    }

    onEventStats(handler) {
        this.on('eventStats', handler);
    }

    onEventLog(handler) {
        this.on('eventLog', handler);
    }

    onNotification(handler) {
        this.on('notification', handler);
    }
}

// Create global SignalR service instance
window.signalRService = new SignalRService();

// Auto-connect when page loads
document.addEventListener('DOMContentLoaded', () => {
    // Start SignalR connection after a short delay
    setTimeout(async () => {
        console.log('Starting SignalR connection...');
        await window.signalRService.connect();
    }, 1000);
});

// Handle page unload
window.addEventListener('beforeunload', () => {
    if (window.signalRService) {
        window.signalRService.disconnect();
    }
});
