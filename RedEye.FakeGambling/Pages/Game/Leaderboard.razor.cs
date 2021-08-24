using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using RedEye.FakeGambling.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class LeaderboardBase : ComponentBase
    {

        public string nametag;
        public string gameInfo;
        [Inject] private IGameService _gameService { get; set; }
        [Inject] private IHubService _hubService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            nametag = _gameService.NameTag;


            _hubService.hubConnection.On<string, string>("ReceiveMessage", (nametag, message) =>
            {

                var encodedMsg = $"{nametag}: {message}";
                if (_gameService.LeaderboardsHistory.Count > 13)
                    _gameService.LeaderboardsHistory.RemoveAt(0);
                _gameService.LeaderboardsHistory.Add(encodedMsg);
                this.InvokeAsync(() => this.StateHasChanged());
            });

            if (!IsHubConnected)
            {
                await _hubService.hubConnection.StartAsync();
            }
        }
        public bool IsHubConnected =>
            _hubService.hubConnection.State == HubConnectionState.Connected;


    }
}
