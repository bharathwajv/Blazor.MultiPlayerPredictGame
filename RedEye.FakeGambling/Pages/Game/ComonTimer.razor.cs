using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using RedEye.FakeGambling.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class ComonTimerBase : GameComponentBase
    {
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] private IGameService _gameService { get; set; }
        [Inject] private IHubService _hubService { get; set; }
        [Inject] IJSRuntime JsRuntime { get; set; }
        public Pos RocketPos = new() { Sec = "0", Mul =0};
        public Pos PlayerPos = new() { Sec = "0", Mul = 0};
        List<decimal> Mult =new();
        List<string> Seconds = new List<string> { "0s" };
        decimal deciTmp;
        int tmpTime = 0;
        bool secTimerRunning = false;
        protected override async Task OnInitializedAsync()
        {
            _hubService.hubConnection.On("ReceiveStartComon", (System.Func<decimal, Task>)(async (crash) =>
            {
                if (!_gameService.IsRunning)
                {
                    //_gameService.JoinPlayerCrashPoint = crash;
                    CommonAnimation();
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
                deciTmp = Decimal.Round(Multiplier, 0);
                if (!Mult.Contains(deciTmp))
                    await AnimateAsync();
                if(!secTimerRunning)
                    SecondsTimer();
                StateHasChanged();
                await Task.Delay(1);
            }
            _gameService.IsRunning = false;
           
            deciTmp = new();
            
            Mult = new();
        }
        public async Task AnimateAsync()
        {
            //if (Mult.Count > 6)
            //    Mult.Remove(Mult.First());
            Mult.Add(deciTmp);
            // await JsRuntime.InvokeVoidAsync("generateLineChart", Mult, Seconds, _gameService.NameTag, RocketPos, PlayerPos);
           await _hubService.hubConnection.SendAsync("SendChart", Mult, Seconds, _gameService.NameTag);//, RocketPos, PlayerPos);
        }
        public async Task SecondsTimer()
        {
            secTimerRunning = true;

            while (_gameService.IsRunning)
            {
                await Task.Delay(1000);
                AddSec();
            }
            Seconds = new List<string> { "0s" };
            tmpTime = 0;
            secTimerRunning = false;

        }
        public void AddSec()
        {
            tmpTime += 1;
            if(Seconds.Count>6)
                Seconds.Remove(Seconds.First());
            Seconds.Add(tmpTime.ToString() + "s");
        }

    }
    public class Pos
    {
        public string Sec { get; set; }
        public double Mul { get; set; }
    }

}
