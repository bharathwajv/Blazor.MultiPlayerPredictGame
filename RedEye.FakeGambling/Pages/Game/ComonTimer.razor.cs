using Hangfire;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using RedEye.FakeGambling.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class ComonTimerBase : GameComponentBase
    {
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] private IGameService _gameService { get; set; }
        [Inject] private IHubService _hubService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            _hubService.hubConnection.On("ReceiveStartComon", (System.Func<decimal, Task>)(async (crash) =>
            {
                if (!_gameService.IsRunning)
                {
                    //_gameService.JoinPlayerCrashPoint = crash;
                    CommonAnimation();
                    SecondsTimer();
                    StateHasChanged();
                }
            }));
        }
        public async Task CommonAnimation()
        {
            _gameService.IsRunning = true;
            for (Multiplier = 1.00M; Multiplier <= 1000000000; Multiplier += 0.01M)
            {
                if (Multiplier >= _gameService.CrashPoint)
                {
                    break;
                }
                StateHasChanged();
                await Task.Delay(1);
            }
            _gameService.IsRunning = false;
        }
        public void SecondsTimer()
        {

        }
    }
}
