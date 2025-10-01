using Microsoft.AspNetCore.SignalR;
using TQM.Backoffice.Application.Contracts.Infrastructure;
using TQM.BackOffice.API.Hubs;

namespace TQM.BackOffice.API.Services;

public class SignalRService : ISignalRService
{
    private readonly IHubContext<KafkaEventsHub> _hubContext;
    private readonly ILogger<SignalRService> _logger;
    private const string KafkaMonitoringGroup = "KafkaMonitoring";

    public SignalRService(IHubContext<KafkaEventsHub> hubContext, ILogger<SignalRService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendKafkaEventAsync(string eventType, object eventData)
    {
        try
        {
            await _hubContext.Clients.Group(KafkaMonitoringGroup)
                .SendAsync("KafkaEventReceived", new
                {
                    eventType = eventType,
                    eventData = eventData,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogDebug("Sent Kafka event via SignalR: {EventType}", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Kafka event via SignalR: {EventType}", eventType);
        }
    }

    public async Task SendEventStatsAsync(object stats)
    {
        try
        {
            await _hubContext.Clients.Group(KafkaMonitoringGroup)
                .SendAsync("EventStatsUpdated", new
                {
                    stats = stats,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogDebug("Sent event stats via SignalR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send event stats via SignalR");
        }
    }

    public async Task SendEventLogAsync(object logEntry)
    {
        try
        {
            await _hubContext.Clients.Group(KafkaMonitoringGroup)
                .SendAsync("EventLogAdded", new
                {
                    logEntry = logEntry,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogDebug("Sent event log via SignalR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send event log via SignalR");
        }
    }

    public async Task SendConnectionStatusAsync(object status)
    {
        try
        {
            await _hubContext.Clients.Group(KafkaMonitoringGroup)
                .SendAsync("ConnectionStatusChanged", new
                {
                    status = status,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogDebug("Sent connection status via SignalR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send connection status via SignalR");
        }
    }

    public async Task SendNotificationAsync(object notification)
    {
        try
        {
            await _hubContext.Clients.Group(KafkaMonitoringGroup)
                .SendAsync("NotificationReceived", new
                {
                    notification = notification,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogDebug("Sent notification via SignalR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification via SignalR");
        }
    }

    public async Task SendProductDataUpdateAsync(string updateType, object productData)
    {
        try
        {
            await _hubContext.Clients.Group(KafkaMonitoringGroup)
                .SendAsync("ProductDataUpdated", new
                {
                    updateType = updateType,
                    productData = productData,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogDebug("Sent product data update via SignalR: {UpdateType}", updateType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send product data update via SignalR: {UpdateType}", updateType);
        }
    }

    public async Task SendOrderDataUpdateAsync(string updateType, object orderData)
    {
        try
        {
            await _hubContext.Clients.Group(KafkaMonitoringGroup)
                .SendAsync("OrderDataUpdated", new
                {
                    updateType = updateType,
                    orderData = orderData,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogDebug("Sent order data update via SignalR: {UpdateType}", updateType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order data update via SignalR: {UpdateType}", updateType);
        }
    }
}
