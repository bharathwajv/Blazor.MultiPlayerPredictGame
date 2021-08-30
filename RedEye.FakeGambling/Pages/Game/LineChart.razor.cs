using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using RedEye.FakeGambling.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedEye.FakeGambling.Pages.Game
{
    public class LineChartBase : ComponentBase
    {
        [Inject] IHubService _hubService { get; set; }
        [Inject] IGameService _game { get; set; }
        public ChartOptions options = new();
        public List<ChartSeries> Series = new()
        {
             new ChartSeries()
             {
                 Name = playerName,
                 Data = new double[] { 0.0, 0.1, 0.2, 0.3, 0.0, 0.1, 0.2, 0.3 }
             }
        };
        public string[] XAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };
        public static string playerName ="";
        protected override void OnInitialized()
        {
            playerName = _game.NameTag;
            //Series.Add(new()
            //{
            //    Name = playerName,
            //    Data = new double[] { 0.0, 0.1, 0.2, 0.3 }
            //});
            //XAxisLabels = ["0s", "1s", "2s", "3s"];
            options.InterpolationOption = InterpolationOption.EndSlope;
            options.YAxisFormat = "c2";

            _hubService.hubConnection.On("ReceiveChart", (List<decimal> mul, List<string> sec, string name) =>
            {
                //if (!_game.IsRunning)
                {
                    var ary = mul.Select(item => Convert.ToDouble(item)).ToArray();
                    Series.First().Data = ary;
                    Series.First().Name = name;
                    //XAxisLabels = sec.ToArray();
                    //_gameService.JoinPlayerCrashPoint = crash;
                    Animation();
                    StateHasChanged();
                }
            });
        }
        public void Animation()
        {

        }
    }
}
