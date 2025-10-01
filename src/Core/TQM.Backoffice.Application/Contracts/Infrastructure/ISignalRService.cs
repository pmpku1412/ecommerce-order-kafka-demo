namespace TQM.Backoffice.Application.Contracts.Infrastructure;

public interface ISignalRService
{
    /// <summary>
    /// Send Kafka event to all connected clients
    /// </summary>
    /// <param name="eventType">Type of Kafka event (e.g., order-created, stock-updated)</param>
    /// <param name="eventData">Event data object</param>
    Task SendKafkaEventAsync(string eventType, object eventData);

    /// <summary>
    /// Send event statistics to all connected clients
    /// </summary>
    /// <param name="stats">Event statistics object</param>
    Task SendEventStatsAsync(object stats);

    /// <summary>
    /// Send event log entry to all connected clients
    /// </summary>
    /// <param name="logEntry">Event log entry object</param>
    Task SendEventLogAsync(object logEntry);

    /// <summary>
    /// Send connection status update to all connected clients
    /// </summary>
    /// <param name="status">Connection status object</param>
    Task SendConnectionStatusAsync(object status);

    /// <summary>
    /// Send notification to all connected clients
    /// </summary>
    /// <param name="notification">Notification object</param>
    Task SendNotificationAsync(object notification);

    /// <summary>
    /// Send product data update to all connected clients
    /// </summary>
    /// <param name="updateType">Type of update (created, updated, stock-changed)</param>
    /// <param name="productData">Product data object</param>
    Task SendProductDataUpdateAsync(string updateType, object productData);

    /// <summary>
    /// Send order data update to all connected clients
    /// </summary>
    /// <param name="updateType">Type of update (created, updated, status-changed)</param>
    /// <param name="orderData">Order data object</param>
    Task SendOrderDataUpdateAsync(string updateType, object orderData);
}
