using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Domain.Events;

namespace TQM.Backoffice.Core.Application.Contracts.Infrastructure;

public interface IKafkaProducer
{
    Task PublishOrderCreatedAsync(OrderResponse order);
    Task PublishProductCreatedAsync(ProductCreatedEvent productEvent);
    Task PublishStockUpdatedAsync(StockUpdatedEvent stockEvent);
    Task PublishNotificationSentAsync(NotificationSentEvent notificationEvent);
    Task PublishStockUpdatedFromOrderAsync(string productId, string productName, int previousStock, int newStock, int orderId);
}
