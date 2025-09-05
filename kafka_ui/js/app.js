// Main Application Controller for E-commerce Kafka Monitor
class App {
    constructor() {
        this.currentSection = 'dashboard';
        this.charts = {};
        this.products = [];
        this.orders = [];
        this.isDarkTheme = false;
        this.refreshInterval = 5000; // 5 seconds
        this.refreshTimer = null;
    }

    // Initialize the application
    async init() {
        console.log('Initializing E-commerce Kafka Monitor...');
        
        // Setup event listeners
        this.setupEventListeners();
        
        // Load initial data
        await this.loadInitialData();
        
        // Setup auto-refresh
        this.startAutoRefresh();
        
        // Initialize charts
        this.initializeCharts();
        
        // Show initial section
        this.showSection('dashboard');
        
        console.log('Application initialized successfully');
    }

    // Setup all event listeners
    setupEventListeners() {
        // Navigation
        document.querySelectorAll('.nav-link[data-section]').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const section = e.target.closest('[data-section]').dataset.section;
                this.showSection(section);
            });
        });

        // Theme toggle
        const themeToggle = document.getElementById('theme-toggle');
        if (themeToggle) {
            themeToggle.addEventListener('click', () => this.toggleTheme());
        }

        // Dashboard refresh
        const refreshDashboard = document.getElementById('refresh-dashboard');
        if (refreshDashboard) {
            refreshDashboard.addEventListener('click', () => this.refreshDashboard());
        }

        // Create order modal
        const submitOrderBtn = document.getElementById('submitOrder');
        if (submitOrderBtn) {
            submitOrderBtn.addEventListener('click', () => this.createOrder());
        }

        // Add order item
        const addOrderItemBtn = document.getElementById('addOrderItem');
        if (addOrderItemBtn) {
            addOrderItemBtn.addEventListener('click', () => this.addOrderItem());
        }

        // Remove order item (delegated event)
        document.addEventListener('click', (e) => {
            if (e.target.closest('.remove-item')) {
                this.removeOrderItem(e.target.closest('.order-item'));
            }
        });

        // Clear logs buttons
        const clearEventLogs = document.getElementById('clear-event-logs');
        if (clearEventLogs) {
            clearEventLogs.addEventListener('click', () => {
                window.kafkaMonitor.clearEventLogs();
            });
        }

        const clearKafkaLogs = document.getElementById('clear-kafka-logs');
        if (clearKafkaLogs) {
            clearKafkaLogs.addEventListener('click', async () => {
                try {
                    // Clear logs via API
                    await window.apiService.clearKafkaEventLogs();
                    window.kafkaMonitor.clearKafkaLogs();
                    window.apiService.showNotification('Kafka logs cleared successfully', 'success');
                } catch (error) {
                    console.error('Error clearing Kafka logs:', error);
                    // Fallback to local clear
                    window.kafkaMonitor.clearKafkaLogs();
                    window.apiService.showNotification('Kafka logs cleared locally', 'warning');
                }
            });
        }

        // Modal events
        const createOrderModal = document.getElementById('createOrderModal');
        if (createOrderModal) {
            createOrderModal.addEventListener('show.bs.modal', () => {
                this.resetOrderForm();
                this.populateProductSelects();
            });
        }
    }

    // Load initial data
    async loadInitialData() {
        try {
            // Load products
            await this.loadProducts();
            
            // Load orders
            await this.loadOrders();
            
            console.log('Initial data loaded successfully');
        } catch (error) {
            console.error('Error loading initial data:', error);
            window.apiService.showNotification('เกิดข้อผิดพลาดในการโหลดข้อมูลเริ่มต้น', 'warning');
        }
    }

    // Load products
    async loadProducts() {
        try {
            const response = await window.apiService.getProducts();
            if (response && response.success) {
                this.products = response.responseObject || [];
                this.renderProducts();
                console.log('Products loaded:', this.products.length);
            }
        } catch (error) {
            console.error('Error loading products:', error);
            // Use mock data as fallback
            const mockResponse = window.apiService.getMockProducts();
            this.products = mockResponse.responseObject || [];
            this.renderProducts();
        }
    }

    // Load orders
    async loadOrders() {
        try {
            const response = await window.apiService.getAllOrders();
            if (response && response.success) {
                this.orders = response.responseObject || [];
                this.renderOrders();
                this.updateDashboardStats();
                console.log('Orders loaded:', this.orders.length);
            }
        } catch (error) {
            console.error('Error loading orders:', error);
            this.orders = [];
            this.renderOrders();
        }
    }

    // Show specific section
    showSection(sectionName) {
        // Hide all sections
        document.querySelectorAll('.content-section').forEach(section => {
            section.style.display = 'none';
        });

        // Show target section
        const targetSection = document.getElementById(`${sectionName}-section`);
        if (targetSection) {
            targetSection.style.display = 'block';
            targetSection.classList.add('fade-in');
        }

        // Update navigation
        document.querySelectorAll('.nav-link').forEach(link => {
            link.classList.remove('active');
        });
        
        const activeLink = document.querySelector(`[data-section="${sectionName}"]`);
        if (activeLink) {
            activeLink.classList.add('active');
        }

        this.currentSection = sectionName;

        // Load section-specific data
        this.loadSectionData(sectionName);
    }

    // Load data specific to current section
    async loadSectionData(sectionName) {
        switch (sectionName) {
            case 'dashboard':
                await this.refreshDashboard();
                break;
            case 'orders':
                await this.loadOrders();
                break;
            case 'products':
                await this.loadProducts();
                break;
            case 'kafka':
                // Kafka data is handled by KafkaMonitor
                break;
            case 'logs':
                // Logs are handled by KafkaMonitor
                break;
        }
    }

    // Render products
    renderProducts() {
        const productsGrid = document.getElementById('products-grid');
        if (!productsGrid) return;

        productsGrid.innerHTML = '';

        this.products.forEach(product => {
            const stockClass = window.apiService.getStockStatusClass(product.stock);
            const productCard = document.createElement('div');
            productCard.className = 'col-lg-4 col-md-6 mb-4';
            
            productCard.innerHTML = `
                <div class="card product-card h-100">
                    <img src="${product.imageUrl}" class="card-img-top product-image" alt="${product.name}">
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title">${product.name}</h5>
                        <p class="card-text flex-grow-1">${product.description}</p>
                        <div class="mt-auto">
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="product-price">${window.apiService.formatCurrency(product.price)}</span>
                                <span class="badge bg-secondary">${product.category}</span>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <small class="product-stock ${stockClass}">
                                    <i class="fas fa-box me-1"></i>
                                    Stock: ${product.stock}
                                </small>
                                <small class="text-muted">ID: ${product.id}</small>
                            </div>
                        </div>
                    </div>
                </div>
            `;

            productsGrid.appendChild(productCard);
        });
    }

    // Render orders
    renderOrders() {
        const ordersTableBody = document.getElementById('orders-tbody');
        if (!ordersTableBody) return;

        ordersTableBody.innerHTML = '';

        if (this.orders.length === 0) {
            ordersTableBody.innerHTML = `
                <tr>
                    <td colspan="6" class="text-center text-muted py-4">
                        <i class="fas fa-shopping-cart fa-2x mb-2"></i><br>
                        ยังไม่มีคำสั่งซื้อ
                    </td>
                </tr>
            `;
            return;
        }

        this.orders.forEach(order => {
            const statusClass = window.apiService.getStatusBadgeClass(order.status);
            const row = document.createElement('tr');
            
            row.innerHTML = `
                <td><strong>#${order.id}</strong></td>
                <td>
                    <div>
                        <strong>${order.customerName}</strong><br>
                        <small class="text-muted">${order.customerId}</small>
                    </div>
                </td>
                <td><strong>${window.apiService.formatCurrency(order.totalAmount)}</strong></td>
                <td><span class="badge ${statusClass}">${order.status}</span></td>
                <td><small>${window.apiService.formatDate(order.createdDate)}</small></td>
                <td>
                    <button class="btn btn-sm btn-outline-primary" onclick="app.viewOrderDetails(${order.id})">
                        <i class="fas fa-eye"></i>
                    </button>
                </td>
            `;

            ordersTableBody.appendChild(row);
        });
    }

    // Update dashboard statistics
    updateDashboardStats() {
        // Total orders
        const totalOrdersElement = document.getElementById('total-orders');
        if (totalOrdersElement) {
            totalOrdersElement.textContent = this.orders.length;
        }

        // Total revenue
        const totalRevenue = this.orders.reduce((sum, order) => sum + (order.totalAmount || 0), 0);
        const totalRevenueElement = document.getElementById('total-revenue');
        if (totalRevenueElement) {
            totalRevenueElement.textContent = window.apiService.formatCurrency(totalRevenue);
        }

        // Active products
        const activeProductsElement = document.getElementById('active-products');
        if (activeProductsElement) {
            activeProductsElement.textContent = this.products.length;
        }

        // Update charts
        this.updateCharts();
    }

    // Initialize charts
    initializeCharts() {
        this.initializeOrdersChart();
        this.initializeRevenueChart();
    }

    // Initialize orders chart
    initializeOrdersChart() {
        const ctx = document.getElementById('ordersChart');
        if (!ctx) return;

        this.charts.orders = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Pending', 'Processing', 'Completed', 'Cancelled'],
                datasets: [{
                    data: [0, 0, 0, 0],
                    backgroundColor: [
                        '#ffc107',
                        '#0dcaf0',
                        '#198754',
                        '#dc3545'
                    ],
                    borderWidth: 2,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    }

    // Initialize revenue chart
    initializeRevenueChart() {
        const ctx = document.getElementById('revenueChart');
        if (!ctx) return;

        this.charts.revenue = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [],
                datasets: [{
                    label: 'Revenue',
                    data: [],
                    borderColor: '#0d6efd',
                    backgroundColor: 'rgba(13, 110, 253, 0.1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return '฿' + value.toLocaleString();
                            }
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    }

    // Update charts with current data
    updateCharts() {
        this.updateOrdersChart();
        this.updateRevenueChart();
    }

    // Update orders chart
    updateOrdersChart() {
        if (!this.charts.orders) return;

        const statusCounts = {
            'Pending': 0,
            'Processing': 0,
            'Completed': 0,
            'Cancelled': 0
        };

        this.orders.forEach(order => {
            if (statusCounts.hasOwnProperty(order.status)) {
                statusCounts[order.status]++;
            }
        });

        this.charts.orders.data.datasets[0].data = Object.values(statusCounts);
        this.charts.orders.update();
    }

    // Update revenue chart
    updateRevenueChart() {
        if (!this.charts.revenue) return;

        // Group orders by date
        const revenueByDate = {};
        this.orders.forEach(order => {
            const date = new Date(order.createdDate).toLocaleDateString('th-TH');
            revenueByDate[date] = (revenueByDate[date] || 0) + order.totalAmount;
        });

        const dates = Object.keys(revenueByDate).sort();
        const revenues = dates.map(date => revenueByDate[date]);

        this.charts.revenue.data.labels = dates;
        this.charts.revenue.data.datasets[0].data = revenues;
        this.charts.revenue.update();
    }

    // Create new order
    async createOrder() {
        const submitBtn = document.getElementById('submitOrder');
        
        try {
            window.apiService.showLoading(submitBtn);

            // Collect form data
            const orderData = this.collectOrderFormData();
            
            // Validate form data
            if (!this.validateOrderForm(orderData)) {
                return;
            }

            // Create order via API
            const response = await window.apiService.createOrder(orderData);
            
            if (response && response.success) {
                // Close modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('createOrderModal'));
                modal.hide();

                // Show success message
                window.apiService.showNotification('สร้างคำสั่งซื้อสำเร็จ!', 'success');

                // Simulate Kafka events
                window.kafkaMonitor.simulateOrderCreated(response.responseObject);

                // Refresh data
                await this.loadOrders();
                
                console.log('Order created successfully:', response.responseObject);
            } else {
                throw new Error('Failed to create order');
            }
        } catch (error) {
            console.error('Error creating order:', error);
            const errorMessage = window.apiService.handleApiError(error);
            window.apiService.showNotification(errorMessage, 'danger');
        } finally {
            window.apiService.hideLoading(submitBtn);
        }
    }

    // Collect order form data
    collectOrderFormData() {
        const orderItems = [];
        document.querySelectorAll('.order-item').forEach(item => {
            const productSelect = item.querySelector('.product-select');
            const quantityInput = item.querySelector('.quantity-input');
            
            if (productSelect.value && quantityInput.value) {
                orderItems.push({
                    productId: productSelect.value,
                    quantity: parseInt(quantityInput.value)
                });
            }
        });

        return {
            customerId: document.getElementById('customerId').value,
            customerName: document.getElementById('customerName').value,
            customerEmail: document.getElementById('customerEmail').value,
            shippingAddress: document.getElementById('shippingAddress').value,
            paymentMethod: document.getElementById('paymentMethod').value,
            orderItems: orderItems
        };
    }

    // Validate order form
    validateOrderForm(orderData) {
        if (!orderData.customerId || !orderData.customerName || !orderData.customerEmail) {
            window.apiService.showNotification('กรุณากรอกข้อมูลลูกค้าให้ครบถ้วน', 'warning');
            return false;
        }

        if (!orderData.shippingAddress || !orderData.paymentMethod) {
            window.apiService.showNotification('กรุณากรอกที่อยู่จัดส่งและวิธีการชำระเงิน', 'warning');
            return false;
        }

        if (orderData.orderItems.length === 0) {
            window.apiService.showNotification('กรุณาเลือกสินค้าอย่างน้อย 1 รายการ', 'warning');
            return false;
        }

        return true;
    }

    // Add order item
    addOrderItem() {
        const orderItemsContainer = document.getElementById('orderItems');
        const newItem = document.createElement('div');
        newItem.className = 'order-item row mb-2';
        
        newItem.innerHTML = `
            <div class="col-md-6">
                <select class="form-select product-select" required>
                    <option value="">Select Product</option>
                </select>
            </div>
            <div class="col-md-3">
                <input type="number" class="form-control quantity-input" placeholder="Quantity" min="1" required>
            </div>
            <div class="col-md-3">
                <button type="button" class="btn btn-outline-danger remove-item">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        `;

        orderItemsContainer.appendChild(newItem);
        this.populateProductSelect(newItem.querySelector('.product-select'));
    }

    // Remove order item
    removeOrderItem(item) {
        const orderItemsContainer = document.getElementById('orderItems');
        if (orderItemsContainer.children.length > 1) {
            item.remove();
        } else {
            window.apiService.showNotification('ต้องมีสินค้าอย่างน้อย 1 รายการ', 'warning');
        }
    }

    // Populate product selects
    populateProductSelects() {
        document.querySelectorAll('.product-select').forEach(select => {
            this.populateProductSelect(select);
        });
    }

    // Populate single product select
    populateProductSelect(select) {
        // Clear existing options except first
        select.innerHTML = '<option value="">Select Product</option>';
        
        this.products.forEach(product => {
            const option = document.createElement('option');
            option.value = product.id;
            option.textContent = `${product.name} - ${window.apiService.formatCurrency(product.price)} (Stock: ${product.stock})`;
            select.appendChild(option);
        });
    }

    // Reset order form
    resetOrderForm() {
        document.getElementById('createOrderForm').reset();
        
        // Reset to single order item
        const orderItemsContainer = document.getElementById('orderItems');
        const firstItem = orderItemsContainer.querySelector('.order-item');
        orderItemsContainer.innerHTML = '';
        orderItemsContainer.appendChild(firstItem);
        
        // Clear the first item
        firstItem.querySelector('.product-select').value = '';
        firstItem.querySelector('.quantity-input').value = '';
    }

    // View order details
    async viewOrderDetails(orderId) {
        try {
            const response = await window.apiService.getOrderById(orderId);
            if (response && response.success) {
                // Show order details in a modal or alert for now
                const order = response.responseObject;
                const details = `
Order ID: ${order.id}
Customer: ${order.customerName} (${order.customerId})
Email: ${order.customerEmail}
Total: ${window.apiService.formatCurrency(order.totalAmount)}
Status: ${order.status}
Created: ${window.apiService.formatDate(order.createdDate)}
Items: ${order.orderItems?.length || 0} items
                `;
                alert(details);
            }
        } catch (error) {
            console.error('Error fetching order details:', error);
            window.apiService.showNotification('ไม่สามารถโหลดรายละเอียดคำสั่งซื้อได้', 'danger');
        }
    }

    // Refresh dashboard
    async refreshDashboard() {
        const refreshBtn = document.getElementById('refresh-dashboard');
        
        try {
            window.apiService.showLoading(refreshBtn);
            
            await this.loadOrders();
            await this.loadProducts();
            
            window.apiService.showNotification('รีเฟรชข้อมูลเรียบร้อย', 'success');
        } catch (error) {
            console.error('Error refreshing dashboard:', error);
            window.apiService.showNotification('เกิดข้อผิดพลาดในการรีเฟรชข้อมูล', 'danger');
        } finally {
            window.apiService.hideLoading(refreshBtn);
        }
    }

    // Toggle theme
    toggleTheme() {
        this.isDarkTheme = !this.isDarkTheme;
        const body = document.body;
        const themeToggle = document.getElementById('theme-toggle');
        
        if (this.isDarkTheme) {
            body.setAttribute('data-theme', 'dark');
            themeToggle.innerHTML = '<i class="fas fa-sun"></i>';
        } else {
            body.removeAttribute('data-theme');
            themeToggle.innerHTML = '<i class="fas fa-moon"></i>';
        }
        
        // Save theme preference
        localStorage.setItem('theme', this.isDarkTheme ? 'dark' : 'light');
    }

    // Load theme preference
    loadThemePreference() {
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'dark') {
            this.toggleTheme();
        }
    }

    // Start auto-refresh
    startAutoRefresh() {
        this.refreshTimer = setInterval(async () => {
            if (this.currentSection === 'dashboard' || this.currentSection === 'orders') {
                await this.loadOrders();
            }
        }, this.refreshInterval);
    }

    // Stop auto-refresh
    stopAutoRefresh() {
        if (this.refreshTimer) {
            clearInterval(this.refreshTimer);
            this.refreshTimer = null;
        }
    }
}

// Create global app instance
window.app = new App();

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', async () => {
    await window.app.init();
    window.app.loadThemePreference();
});

// Cleanup on page unload
window.addEventListener('beforeunload', () => {
    window.app.stopAutoRefresh();
    window.kafkaMonitor.stopMonitoring();
});
