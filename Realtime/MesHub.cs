
using Microsoft.AspNet.SignalR;
namespace MesEnterprise.Realtime
{
    public class MesHub : Hub
    {
        public Task Broadcast(object dto) => Clients.All.SendAsync("broadcast", dto);
    }
}
