using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using RedEye.FakeGambling.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class LeaderboardBase : ComponentBase
    {

        public List<string> messages = new List<string>();
        public string nametag;
        public string gameInfo;
        [Inject] private IGameService _gameService { get; set; }
        [Inject] private HubService _hubService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            nametag = _gameService.NameTag;


            _hubService.hubConnection.On<string, string>("ReceiveMessage", (nametag, message) =>
            {
                var encodedMsg = $"{nametag}: {message}";
                messages.Add(encodedMsg);
                StateHasChanged();
            });

            if (!IsHubConnected)
            {
                await _hubService.hubConnection.StartAsync();
            }
        }

        public async Task Send()
        {
            gameInfo = _gameService.CrashPoint.ToString();
            await _hubService.hubConnection.SendAsync("SendMessage", nametag, gameInfo);
        }

        public bool IsHubConnected =>
            _hubService.hubConnection.State == HubConnectionState.Connected;


    }
}
