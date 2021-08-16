using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendCrash(string user, decimal message)
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("ReceiveCrash", user, message);
        }
        public async Task SendOnlineCrash(string user, decimal message)
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("ReceiveOnlineCrash", user, message);
        }
    }
}
