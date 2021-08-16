using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Data
{
    public class OnlineService
    {
        private HubService _hubService { get; set; }
        private IGameService _gameService { get; set; }
        public OnlineService(HubService HubService, IGameService gameService)
        {
            _hubService = HubService;
            _gameService = gameService;
        }
        public async Task UpdateCrashAsync()
        {
            _gameService.NewCrashPoint();
            await _hubService.hubConnection.SendAsync("SendOnlineCrash", "Online", _gameService.JoinPlayerCrashPoint);
        }
    }
}
