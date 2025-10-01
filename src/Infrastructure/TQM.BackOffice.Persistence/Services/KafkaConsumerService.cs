using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TQM.Backoffice.Application.Contracts.Infrastructure;
using TQM.Backoffice.Domain.Events;
using TQM.Backoffice.Core.Application.Contracts.Persistence;
using System.Collections.Concurrent;

namespace TQM.BackOffice.Persistence.Services;

public class KafkaEventLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Topic { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Level { get; set; } = "info";
    public object? EventData { get; set; }
}

public class KafkaEventStatistics
{
    public Dictionary<string, int> EventCounts { get; set; } = new();
    public int TotalEvents { get; set; }
    public List<KafkaEventLog> RecentLogs { get; set; } = new();
    public bool IsMonitoring { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class KafkaConsumerService : IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _bootstrapServers;
    private Task? _executingTask;
    private CancellationTokenSource? _stoppingCts;
    
    // Event tracking
    private readonly ConcurrentQueue<KafkaEventLog> _eventLogs = new();
    private readonly ConcurrentDictionary<string, int> _eventCounts = new();
    private readonly int _maxLogEntries = 100;

    public KafkaConsumerService(IConfiguration configuration, ILogger<KafkaConsumerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        var groupId = configuration["Kafka:GroupId"] ?? "ecommerce-order-group";

        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_stoppingCts.Token);
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
            return;

        try
        {
            _stoppingCts?.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(new[] { "order-created", "product-created", "stock-updated", "notification-sent" });

        _logger.LogInformation("Kafka Consumer Service started. Listening to topics: order-created, product-created, stock-updated, notification-sent");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    
                    if (consumeResult?.Message != null)
                    {
                        await ProcessMessage(consumeResult.Topic, consumeResult.Message.Key, consumeResult.Message.Value);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in Kafka consumer");
                }
            }
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessMessage(string topic, string key, string value)
    {
        _logger.LogInformation($"Processing message from topic: {topic}, Key: {key}");

        try
        {
            // Track event count
            _eventCounts.AddOrUpdate(topic, 1, (k, v) => v + 1);

            switch (topic)
            {
                case "order-created":
                    await ProcessOrderCreatedEvent(value);
                    break;
                case "product-created":
                    await ProcessProductCreatedEvent(value);
                    break;
                case "stock-updated":
                    await ProcessStockUpdatedEvent(value);
                    break;
                case "notification-sent":
                    await ProcessNotificationSentEvent(value);
                    break;
                default:
                    _logger.LogWarning($"Unknown topic: {topic}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing message from topic {topic}");
            AddEventLog(topic, $"Error processing message: {ex.Message}", "error", null);
        }
    }

    private async Task ProcessOrderCreatedEvent(string message)
    {
        var orderEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(message);
        if (orderEvent != null)
        {
            _logger.LogInformation($"Processing Order Created Event for Order ID: {orderEvent.OrderId}");
            
            AddEventLog("order-created", $"Order {orderEvent.OrderId} created for {orderEvent.CustomerName} - Amount: {orderEvent.TotalAmount:C}", "success", orderEvent);
            
            // Simulate stock update processing
            await Task.Delay(100);
            _logger.LogInformation($"Stock updated for Order ID: {orderEvent.OrderId}");
            
            // Simulate notification processing
            await Task.Delay(100);
            _logger.LogInformation($"Notification sent for Order ID: {orderEvent.OrderId} to {orderEvent.CustomerEmail}");
            
            // Simulate analytics processing
            await Task.Delay(50);
            _logger.LogInformation($"Analytics data recorded for Order ID: {orderEvent.OrderId}, Amount: {orderEvent.TotalAmount:C}");
        }
    }

    private async Task ProcessProductCreatedEvent(string message)
    {
        var productEvent = JsonConvert.DeserializeObject<ProductCreatedEvent>(message);
        if (productEvent != null)
        {
            _logger.LogInformation($"Processing Product Created Event for Product: {productEvent.ProductName}");
            
            AddEventLog("product-created", 
                $"New product created: {productEvent.ProductName} - Price: {productEvent.Price:C} - Initial Stock: {productEvent.InitialStock}", 
                "success", productEvent);
            
            try
            {
                // Use scoped service to verify product creation
                using var scope = _serviceProvider.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                
                // Verify the product exists
                var product = productService.GetProductById(productEvent.ProductId);
                if (product != null)
                {
                    _logger.LogInformation($"Product creation confirmed: {productEvent.ProductName} (ID: {productEvent.ProductId})");
                    
                    // Here you could add additional processing like:
                    // - Send notifications to administrators
                    // - Update analytics/reporting data
                    // - Trigger inventory management processes
                    // - Update search indexes
                    
                    // Simulate processing time
                    await Task.Delay(50);
                    
                    // Send real-time product data update via SignalR
                    await SendProductDataUpdate("created", product, $"New product created: {productEvent.ProductName}");
                    
                    _logger.LogInformation($"Product creation processing completed for: {productEvent.ProductName}");
                }
                else
                {
                    _logger.LogWarning($"Product {productEvent.ProductId} not found after creation event");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing product creation for Product {productEvent.ProductId}");
            }
        }
    }

    private async Task ProcessStockUpdatedEvent(string message)
    {
        var stockEvent = JsonConvert.DeserializeObject<StockUpdatedEvent>(message);
        if (stockEvent != null)
        {
            _logger.LogInformation($"Processing Stock Updated Event for Product: {stockEvent.ProductName}, Previous: {stockEvent.PreviousStock}, New: {stockEvent.NewStock}");
            
            try
            {
                // Use scoped service to update product data
                using var scope = _serviceProvider.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                
                // Verify the product exists
                var product = productService.GetProductById(stockEvent.ProductId);
                if (product != null)
                {
                    // Log the stock change for analytics/audit purposes
                    _logger.LogInformation($"Stock change confirmed for Product {stockEvent.ProductName}: {stockEvent.PreviousStock} → {stockEvent.NewStock} (Change: {stockEvent.QuantityChanged})");
                    
                    // Here you could add additional processing like:
                    // - Send notifications if stock is low
                    // - Update analytics/reporting data
                    // - Trigger reorder processes
                    
                    if (stockEvent.NewStock <= 5) // Low stock threshold
                    {
                        _logger.LogWarning($"LOW STOCK ALERT: Product {stockEvent.ProductName} has only {stockEvent.NewStock} items remaining!");
                    }
                    
                    // Simulate processing time
                    await Task.Delay(50);
                    
                    // Send real-time product data update via SignalR
                    await SendProductDataUpdate("stock-updated", new
                    {
                        product = product,
                        previousStock = stockEvent.PreviousStock,
                        newStock = stockEvent.NewStock,
                        quantityChanged = stockEvent.QuantityChanged,
                        reason = stockEvent.Reason
                    }, $"Stock updated for {stockEvent.ProductName}: {stockEvent.PreviousStock} → {stockEvent.NewStock}");
                    
                    _logger.LogInformation($"Stock update processing completed for Product: {stockEvent.ProductName}");
                }
                else
                {
                    _logger.LogWarning($"Product {stockEvent.ProductId} not found during stock update processing");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing stock update for Product {stockEvent.ProductId}");
            }
        }
    }

    private async Task ProcessNotificationSentEvent(string message)
    {
        var notificationEvent = JsonConvert.DeserializeObject<NotificationSentEvent>(message);
        if (notificationEvent != null)
        {
            _logger.LogInformation($"Processing Notification Sent Event for Order: {notificationEvent.OrderId}, Type: {notificationEvent.NotificationType}");
            
            AddEventLog("notification-sent", $"{notificationEvent.NotificationType} sent to customer for order {notificationEvent.OrderId}", "success", notificationEvent);
            
            await Task.Delay(50);
        }
    }

    // Public methods for API access
    public List<KafkaEventLog> GetRecentEvents()
    {
        return _eventLogs.Take(50).ToList();
    }

    public KafkaEventStatistics GetEventStatistics()
    {
        var totalEvents = _eventCounts.Values.Sum();
        return new KafkaEventStatistics
        {
            EventCounts = new Dictionary<string, int>(_eventCounts),
            TotalEvents = totalEvents,
            RecentLogs = _eventLogs.Take(10).ToList(),
            IsMonitoring = _executingTask != null && !_executingTask.IsCompleted,
            LastUpdated = DateTime.UtcNow
        };
    }

    public void ClearEventLogs()
    {
        while (_eventLogs.TryDequeue(out _)) { }
        _eventCounts.Clear();
        AddEventLog("system", "Event logs cleared", "info", null);
    }

    // Private helper methods
    private void AddEventLog(string topic, string message, string level, object? eventData)
    {
        var logEntry = new KafkaEventLog
        {
            Topic = topic,
            EventType = GetEventTypeFromTopic(topic),
            Message = message,
            Level = level,
            EventData = eventData
        };

        _eventLogs.Enqueue(logEntry);

        // Keep only recent logs
        while (_eventLogs.Count > _maxLogEntries)
        {
            _eventLogs.TryDequeue(out _);
        }

        // Send real-time updates via SignalR
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var signalRService = scope.ServiceProvider.GetService<ISignalRService>();
                
                if (signalRService != null)
                {
                    // Send the specific event
                    await signalRService.SendKafkaEventAsync(topic, eventData ?? new { message });
                    
                    // Send updated event log
                    await signalRService.SendEventLogAsync(logEntry);
                    
                    // Send updated statistics
                    var stats = GetEventStatistics();
                    await signalRService.SendEventStatsAsync(stats);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send SignalR update for event: {Topic}", topic);
            }
        });
    }

    private async Task SendProductDataUpdate(string updateType, object productData, string message)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var signalRService = scope.ServiceProvider.GetService<ISignalRService>();
            
            if (signalRService != null)
            {
                await signalRService.SendProductDataUpdateAsync(updateType, new
                {
                    product = productData,
                    message = message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send product data update via SignalR");
        }
    }

    private string GetEventTypeFromTopic(string topic)
    {
        return topic switch
        {
            "order-created" => "OrderCreatedEvent",
            "product-created" => "ProductCreatedEvent",
            "stock-updated" => "StockUpdatedEvent",
            "notification-sent" => "NotificationSentEvent",
            "system" => "SystemEvent",
            _ => "UnknownEvent"
        };
    }

    public void Dispose()
    {
        _stoppingCts?.Cancel();
        _consumer?.Dispose();
        _stoppingCts?.Dispose();
    }
}
