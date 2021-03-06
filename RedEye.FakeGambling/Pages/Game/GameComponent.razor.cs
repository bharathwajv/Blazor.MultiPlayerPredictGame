using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using RedEye.FakeGambling.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class GameComponentBase : ComponentBase
    {
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] private IGameService _gameService { get; set; }
        [Inject] private IHubService _hubService { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        public bool isCreateLobby { get; set; }
        [Parameter] public bool isMutiplayer { get; set; } = false;
        public decimal Multiplier { get; set; } = 1.00M;
        public Severity MultiplierColor { get; set; } = Severity.Normal;
        
        public string HubConnectionId { get; set; }

        public PlayerInfo playerInfo;
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
            _gameService.IsRunning = isRunning;
        }
        public void CalculateCurrentCrash()
        {
            CrashPointList.Add(_gameService.CrashPoint);
            if (CrashPointList.Count >= 10)
            {
                CrashPointList.Remove(CrashPointList.First());
            }
        }
        public void MaskeItHalf() =>
            playerInfo.BetAmount /= 2;
        public void MaskeTwice() =>
            playerInfo.BetAmount *= 2;
        public void SetAmountMax() =>
            playerInfo.BetAmount = _gameService.UserCash;

        public async Task OnValidSubmitAsync()
        {

            if (_gameService.IsRunning)
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
                if (_gameService.IsRunning)
                {
                    Snackbar.Add("Wait for next round", Severity.Info, config => { config.HideIcon = true; });
                }
                else if (isCreateLobby && isMutiplayer)
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
                {
                    _gameService.NewCrashPoint();
                    await _hubService.hubConnection.SendAsync("SendStartComon", _gameService.JoinPlayerCrashPoint);
                }
                if (isCreateLobby && isMutiplayer)
                    await _hubService.hubConnection.SendAsync("SendCrash", playerInfo.NameTag, _gameService.CrashPoint);
                await Animate();
            }
            else if (_gameService.UserCash < _gameService.BetAmount)
            {
                Snackbar.Add("You do not have enough money to make that bet!", Severity.Error, config => { config.HideIcon = true; });
                _gameService.BetAmount = 0;
            }
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
                    MultiplierColor = Severity.Success;
                    _gameService.UserCash += _gameService.BetAmount * Multiplier;
                    string message = "Won "+ (_gameService.UserCash- playerInfo.PlayerBal).ToString() +"$";
                    await SendInfos(message);
                    playerInfo.PlayerBal = _gameService.UserCash;
                    break;
                }
                else if (Multiplier >= _gameService.CrashPoint)
                {
                    MultiplierColor = Severity.Error;
                    string message = "Lost " + (playerInfo.BetAmount).ToString() + "$";
                    await SendInfos(message);
                    break;
                }
                else if (_gameService.Cashit == true)
                {
                    MultiplierColor = Severity.Success;
                    _gameService.UserCash += _gameService.BetAmount * Multiplier;
                    string message = "Won " + (_gameService.UserCash - playerInfo.PlayerBal).ToString() + "$";
                    await SendInfos(message);
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
            do
            {
                await Task.Delay(1);
            } while (_gameService.IsRunning);
            
            await Task.Delay(3000);
            await _hubService.hubConnection.SendAsync("SendDeleteChart");
            CalculateCurrentCrash();
            Multiplier = 1.00M;
            MultiplierColor = Severity.Normal;
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
        public async Task SendInfos(string message)
        {
            await _hubService.hubConnection.SendAsync("SendMessage", playerInfo.NameTag, message);
            await _hubService.hubConnection.SendAsync("SendPlayerCrash", playerInfo.NameTag, Multiplier);
        }
        public void OpenChat()
        {
            _ = DialogService.Show<Chat>().Result;

        }
    }
}
