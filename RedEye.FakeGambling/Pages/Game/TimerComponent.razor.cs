using Hangfire;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using RedEye.FakeGambling.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class TimerComponentBase : OnlineGameComponentBase
    {
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] private IGameService _gameService { get; set; }
        [Inject] private HubService _hubService { get; set; }
        public Severity OnlineMultiplierColor { get; set; } = Severity.Normal;
        protected override async Task OnInitializedAsync()
        {
            _hubService.hubConnection.On<string, decimal>("ReceiveOnlineCrash", async (nametag, crash) =>
            {
                if (!isGameRunning)
                {
                    var encodedMsg = $"{_hubService.hubConnection.ConnectionId} : {crash}";
                    _gameService.JoinPlayerCrashPoint = crash;
                    isTimetoBet = true;
                    for (GameStartsIn = 5M; GameStartsIn > 0; GameStartsIn = GameStartsIn - 0.01M)
                    {
                        StateHasChanged();
                        await Task.Delay(1);
                    }
                    base.isTimetoBet = false;
                    CommonAnimation();
                    StateHasChanged();
                }
            });
        }
        public async Task CommonAnimation()
        {
            isGameRunning = true;
            for (OnlineMultiplier = 1.00M; OnlineMultiplier <= 1000000000; OnlineMultiplier = OnlineMultiplier + 0.01M)
            {
                if (OnlineMultiplier >= _gameService.CrashPoint)
                {
                    OnlineMultiplierColor = Severity.Error;
                    break;
                }
                StateHasChanged();
                await Task.Delay(1);
            }
            isGameRunning = false;
        }
    }
}
