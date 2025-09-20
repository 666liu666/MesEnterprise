using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MesEnterprise.Infrastructure.Realtime;

[Authorize]
public class MesHub : Hub
{
    public async Task BroadcastNotificationAsync(string title, string message)
    {
        await Clients.All.SendAsync("notification", new { title, message, timestamp = DateTimeOffset.UtcNow });
    }

    public async Task SendWorkOrderUpdateAsync(string workOrderNumber, string status)
    {
        await Clients.Groups($"wo:{workOrderNumber}").SendAsync("workOrderStatus", new { workOrderNumber, status, timestamp = DateTimeOffset.UtcNow });
    }
}
