using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using RedEye.FakeGambling.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class GameComponentBase : ComponentBase
    {
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] private IGameService _gameService { get; set; }
        [Inject] private HubService _hubService { get; set; }
        public bool isCreateLobby { get; set; }
        [Parameter] public bool isMutiplayer { get; set; } = false;
        public decimal Multiplier { get; set; } = 1.00M;
        public Severity MultiplierColor { get; set; } = Severity.Normal;
        
        public string HubConnectionId { get; set; }

        public PlayerInfo playerInfo;
        public bool isRunning = false;
        public bool disableInputs = false;
        public bool cashOutDisable = true;
        public bool placeBetDisable = false;
        public List<decimal> CrashPointList = new();
        public bool IsHubConnected =>
         _hubService.hubConnection.State == HubConnectionState.Connected;
        protected override async Task OnInitializedAsync()
        {
            playerInfo = new() { BetAmount = 1, AutoCashOut = 1000, PlayerBal = 10, NameTag = _gameService.NameTag };
            isCreateLobby = _gameService.isCreatePlayer;
            
            _hubService.hubConnection.On<string, decimal>("ReceiveCrash", (nametag, crash) =>
            {
                var encodedMsg = $"{_hubService.hubConnection.ConnectionId} : {crash}";
                _gameService.JoinPlayerCrashPoint = crash;
                this.InvokeAsync(() => this.StateHasChanged());
            });
            if (_hubService.hubConnection.State != HubConnectionState.Connected)
            {
                await _hubService.hubConnection.StartAsync();
            }
            HubConnectionId = _hubService.hubConnection.ConnectionId;
            StateHasChanged();
        }
       
        public void SetisRunning(bool isRunning)
        {
            this.isRunning = isRunning;
        }
        public void CalculateCurrentCrash()
        {
            CrashPointList.Add(_gameService.CrashPoint);
        }
        public async Task OnValidSubmitAsync()
        {

            if (isRunning)
            {
                //cashout
                _gameService.Cashit = true;
                disableInputs = false;
                cashOutDisable = true;
                placeBetDisable = false;
                MultiplierColor = Severity.Info;
            }
            else
            {
                //place bet
                if (isCreateLobby && isMutiplayer)
                {
                    await PlaceBet();
                }
                else if(!isCreateLobby && isMutiplayer)
                {
                    if (_gameService.LastJoinPlayerCrashPoint == _gameService.JoinPlayerCrashPoint || _gameService.JoinPlayerCrashPoint == 0)
                        Snackbar.Add("Wait for host", Severity.Info, config => { config.HideIcon = true; });
                    else
                    {
                        _gameService.LastJoinPlayerCrashPoint = _gameService.JoinPlayerCrashPoint;
                        await PlaceBet();
                    }
                }
                else
                { //solo
                    await PlaceBet();
                }
            }
            StateHasChanged();
        }
        public async Task PlaceBet()
        {
            _gameService.BetAmount = playerInfo.BetAmount;
            if (_gameService.UserCash >= _gameService.BetAmount)
            {
                disableInputs = true;
                cashOutDisable = false;
                placeBetDisable = true;
                _gameService.UserCash -= _gameService.BetAmount;
                playerInfo.PlayerBal = _gameService.UserCash;
                if (isCreateLobby || !isMutiplayer)
                    _gameService.NewCrashPoint();
                if (isCreateLobby && isMutiplayer)
                    await _hubService.hubConnection.SendAsync("SendCrash", playerInfo.NameTag, _gameService.CrashPoint);
                if (CrashPointList.Count >= 18)
                {
                    CrashPointList.Clear();
                }
                await Animate();
            }
            else if (_gameService.UserCash < _gameService.BetAmount)
            {
                Snackbar.Add("You do not have enough money to make that bet!", Severity.Error, config => { config.HideIcon = true; });
                _gameService.BetAmount = 0;
            }
            isRunning = true;
        }
        public async Task Animate()
        {
            if (!isMutiplayer)
            {
                await AnimateInternal();
            }
            else
            {
                if (isCreateLobby)
                {
                    await AnimateInternal();
                }
                else
                {
                        await AnimateInternal();
                }
            }
        }
        public async Task AnimateInternal()
        {
            for (Multiplier = 1.00M; Multiplier <= 1000000000; Multiplier = Multiplier + 0.01M)
            {
                if (Multiplier >= playerInfo.AutoCashOut)
                {
                    isRunning = false;
                    MultiplierColor = Severity.Success;
                    _gameService.UserCash += _gameService.BetAmount * Multiplier;
                    string message = "Won "+ (_gameService.UserCash- playerInfo.PlayerBal).ToString() +"$";
                    await _hubService.hubConnection.SendAsync("SendMessage", playerInfo.NameTag, message);
                    playerInfo.PlayerBal = _gameService.UserCash;
                    break;
                }
                else if (Multiplier >= _gameService.CrashPoint)
                {
                    isRunning = false;
                    MultiplierColor = Severity.Error;
                    string message = "Lost " + (playerInfo.BetAmount).ToString() + "$";
                    await _hubService.hubConnection.SendAsync("SendMessage", playerInfo.NameTag, message);
                    break;
                }
                else if (_gameService.Cashit == true)
                {
                    MultiplierColor = Severity.Success;
                    _gameService.UserCash += _gameService.BetAmount * Multiplier;
                    string message = "Won " + (_gameService.UserCash - playerInfo.PlayerBal).ToString() + "$";
                    await _hubService.hubConnection.SendAsync("SendMessage", playerInfo.NameTag, message);
                    playerInfo.PlayerBal = _gameService.UserCash;
                    break;
                }
                StateHasChanged();
                await Task.Delay(1);
            }
            disableInputs = true;
            placeBetDisable = true;
            cashOutDisable = true;
            StateHasChanged();
            await Task.Delay(3000);
            CalculateCurrentCrash();
            Multiplier = 1.00M;
            MultiplierColor = Severity.Normal;
            isRunning = false;
            disableInputs = false;
            placeBetDisable = false;
            cashOutDisable = true;
            _gameService.Cashit = false;
            if (_gameService.UserCash <= 0)
            {
                Snackbar.Add("You Lost! No Cash Available", Severity.Error, config => { config.HideIcon = true; });
            }
            StateHasChanged();
        }
        public void Dispose()
        {
            //clear connection
        }
    }
}
