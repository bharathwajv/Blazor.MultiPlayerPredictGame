using Microsoft.AspNetCore.Components;
using MudBlazor;
using RedEye.FakeGambling.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Pages.Game
{
    public class GameComponentBase : ComponentBase, IDisposable
    {
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] private IGameService _gameService { get; set; }

        [Parameter] public bool isMutiplayer { get; set; } = false;
        public decimal Multiplier { get; set; } = 1.00M;
        public Severity MultiplierColor { get; set; } = Severity.Normal;

        public PlayerInfo playerInfo = new() { BetAmount = 1, AutoCashOut = 1000, PlayerBal = 10 };
        public bool isRunning = false;
        public bool disableInputs = false;
        public bool cashOutDisable = true;
        public bool placeBetDisable = false;
        public List<decimal> CrashPointList = new();
        protected override void OnInitialized()
        {
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
                _gameService.BetAmount = playerInfo.BetAmount;
                if (_gameService.UserCash >= _gameService.BetAmount)
                {
                    disableInputs = true;
                    cashOutDisable = false;
                    placeBetDisable = true;
                    _gameService.UserCash -= _gameService.BetAmount;
                    playerInfo.PlayerBal = _gameService.UserCash;
                    _gameService.NewCrashPoint();
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
            StateHasChanged();
        }

        public async Task Animate()
        {
            for (Multiplier = 1.00M; Multiplier <= 1000000000; Multiplier = Multiplier + 0.01M)
            {
                if (Multiplier >= playerInfo.AutoCashOut)
                {
                    isRunning = false;
                    MultiplierColor = Severity.Success;
                    _gameService.UserCash += _gameService.BetAmount * Multiplier;
                    playerInfo.PlayerBal = _gameService.UserCash;
                    break;
                }
                else if (Multiplier >= _gameService.CrashPoint)
                {
                    isRunning = false;
                    MultiplierColor = Severity.Error;
                    break;
                }
                else if (_gameService.Cashit == true)
                {
                    MultiplierColor = Severity.Success;
                    _gameService.UserCash += _gameService.BetAmount * Multiplier;
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
                Values.ForcedExit = true;
            }
            StateHasChanged();
        }
        public async void Dispose()
        {
           //clear connection
        }

    }
}
