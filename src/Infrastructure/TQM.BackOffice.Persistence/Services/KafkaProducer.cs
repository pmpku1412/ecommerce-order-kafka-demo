using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TQM.Backoffice.Core.Application.Contracts.Infrastructure;
using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Domain.Events;

namespace TQM.BackOffice.Persistence.Services;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly string _bootstrapServers;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

        var config = new ProducerConfig
        {
            BootstrapServers = _bootstrapServers,
            ClientId = "ecommerce-order-producer"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishOrderCreatedAsync(OrderResponse order)
    {
        try
        {
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                TotalAmount = order.TotalAmount,
                CreatedDate = order.CreatedDate,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                OrderItems = order.OrderItems.Select(item => new OrderItemEvent
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };

            var message = JsonConvert.SerializeObject(orderEvent);
            var kafkaMessage = new Message<string, string>
            {
                Key = order.Id.ToString(),
                Value = message
            };

            var result = await _producer.ProduceAsync("order-created", kafkaMessage);
            _logger.LogInformation($"Order created event published to Kafka. Topic: order-created, Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to publish order created event for order {order.Id}");
            throw;
        }
    }

    public async Task PublishCreateProductAsync(StockUpdatedEvent stockEvent)
    {
        try
        {
            var message = JsonConvert.SerializeObject(stockEvent);
            var kafkaMessage = new Message<string, string>
            {
                Key = stockEvent.ProductId,
                Value = message
            };

            var result = await _producer.ProduceAsync("stock-updated", kafkaMessage);
            _logger.LogInformation($"Product Created event published to Kafka. Topic: stock-updated, Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to publish stock updated event for product {stockEvent.ProductId}");
            throw;
        }
    }

    public async Task PublishStockUpdatedAsync(StockUpdatedEvent stockEvent)
    {
        try
        {
            var message = JsonConvert.SerializeObject(stockEvent);
            var kafkaMessage = new Message<string, string>
            {
                Key = stockEvent.ProductId,
                Value = message
            };

            var result = await _producer.ProduceAsync("stock-updated", kafkaMessage);
            _logger.LogInformation($"Stock updated event published to Kafka. Topic: stock-updated, Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to publish stock updated event for product {stockEvent.ProductId}");
            throw;
        }
    }

    public async Task PublishNotificationSentAsync(NotificationSentEvent notificationEvent)
    {
        try
        {
            var message = JsonConvert.SerializeObject(notificationEvent);
            var kafkaMessage = new Message<string, string>
            {
                Key = notificationEvent.OrderId.ToString(),
                Value = message
            };

            var result = await _producer.ProduceAsync("notification-sent", kafkaMessage);
            _logger.LogInformation($"Notification sent event published to Kafka. Topic: notification-sent, Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to publish notification sent event for order {notificationEvent.OrderId}");
            throw;
        }
    }

    public async Task PublishStockUpdatedFromOrderAsync(string productId, string productName, int previousStock, int newStock, int orderId)
    {
        try
        {
            var stockEvent = new StockUpdatedEvent
            {
                ProductId = productId,
                ProductName = productName,
                PreviousStock = previousStock,
                NewStock = newStock,
                QuantityChanged = newStock - previousStock,
                UpdatedDate = DateTime.UtcNow,
                Reason = "ORDER_CREATED",
                OrderId = orderId
            };

            await PublishStockUpdatedAsync(stockEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to publish stock updated event for product {productId} from order {orderId}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
