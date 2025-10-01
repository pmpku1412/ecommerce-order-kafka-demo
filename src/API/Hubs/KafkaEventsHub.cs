using Microsoft.AspNetCore.SignalR;

namespace TQM.BackOffice.API.Hubs;

public class KafkaEventsHub : Hub
{
    private const string KafkaMonitoringGroup = "KafkaMonitoring";

    public async Task JoinKafkaMonitoring()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, KafkaMonitoringGroup);
        await Clients.Caller.SendAsync("JoinedKafkaMonitoring", new { 
            connectionId = Context.ConnectionId,
            message = "Successfully joined Kafka monitoring group"
        });
    }

    public async Task LeaveKafkaMonitoring()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, KafkaMonitoringGroup);
        await Clients.Caller.SendAsync("LeftKafkaMonitoring", new { 
            connectionId = Context.ConnectionId,
            message = "Left Kafka monitoring group"
        });
    }

    public override async Task OnConnectedAsync()
    {
        // Auto-join monitoring group when connected
        await JoinKafkaMonitoring();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, KafkaMonitoringGroup);
        await base.OnDisconnectedAsync(exception);
    }
}
