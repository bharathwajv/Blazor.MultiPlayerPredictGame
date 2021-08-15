﻿using Microsoft.AspNetCore.SignalR;
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
    }
}