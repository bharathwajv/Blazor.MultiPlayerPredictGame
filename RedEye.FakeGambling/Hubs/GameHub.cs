using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendChatMessage(string user, string message)
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("ReceiveChatMessage", user, message);
        }
        public async Task SendMessage(string user, string message)
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendPlayerCrash(string user, decimal crash)
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("ReceivePlayerCrash", user, crash);
        }
        public async Task SendDeleteChart()
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("DeleteChart");
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
        public async Task SendStartComon(decimal message)
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("ReceiveStartComon", message);
        }

        public async Task SendChart(List<decimal> mul, List<string> sec, string name)
        {
            //var sameConnection = Clients.Clients(user);
            await Clients.All.SendAsync("ReceiveChart", mul, sec, name);
        }
    }
}
