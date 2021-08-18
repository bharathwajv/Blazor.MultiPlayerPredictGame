using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using RedEye.FakeGambling.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class ChatBase : ComponentBase
    {

        [Inject] private IGameService _gameService { get; set; }
        [Inject] private HubService _hubService { get; set; }

        public ChatInfo info = new();
        protected override async Task OnInitializedAsync()
        {
            info.Name = _gameService.NameTag;


            _hubService.hubConnection.On<string, string>("ReceiveChatMessage", (nametag, message) =>
            {
                var encodedMsg = $"{nametag}: {message}";
                _gameService.ChatMessages.Add(encodedMsg);
                this.InvokeAsync(() => this.StateHasChanged());
            });

            if (!IsHubConnected)
            {
                await _hubService.hubConnection.StartAsync();
            }
        }

        public async Task Send()
        {
            if (_gameService.ChatMessages.Count > 13)
                _gameService.ChatMessages.RemoveAt(0);
            await _hubService.hubConnection.SendAsync("SendChatMessage", info.Name, info.Message);
            info.Message = "";
            StateHasChanged();
        }

        public bool IsHubConnected =>
            _hubService.hubConnection.State == HubConnectionState.Connected;


    }
}
