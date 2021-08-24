using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Data
{
    public class OnlineService : IOnlineService
    {
        private IHubService _hubService { get; set; }
        private IGameService _gameService { get; set; }
        public OnlineService(IHubService HubService, IGameService gameService)
        {
            _hubService = HubService;
            _gameService = gameService;
        }
        public async Task UpdateCrashAsync()
        {
            //generate new crash point
            _gameService.NewCrashPoint();
            //send to all connections
            await _hubService.hubConnection.SendAsync("SendOnlineCrash", "Online", _gameService.JoinPlayerCrashPoint);
        }
    }
}
