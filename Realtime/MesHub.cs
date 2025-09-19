using Microsoft.AspNetCore.SignalR;

namespace MesEnterprise.Realtime;

public class MesHub : Hub
{
    public async Task BroadcastAsync(object payload) => await Clients.All.SendAsync("broadcast", payload);
}
