using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NetMQ.Web
{
    public class NotifHub : Hub
    {

        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification",message);
        }
    }
}
