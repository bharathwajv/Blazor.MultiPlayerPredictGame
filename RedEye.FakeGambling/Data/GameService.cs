using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Data
{
    public class GameService : IGameService
    {
        public bool Cashit { get; set; } = false;
        public decimal UserCash { get; set; }
        public decimal BetAmount { get; set; }
        public int RandomNumber { get; set; }
        public decimal CrashPoint { get; set; }
        public decimal JoinPlayerCrashPoint { get; set; }
        public decimal LastJoinPlayerCrashPoint { get; set; }
        public string NameTag { get; set; }

        public bool isCreatePlayer { get; set; }

        public void NewCrashPoint()
        {
            // Use RNGCryptoService function to create a very large random number, making the multiplier small.
            RandomNumber = Values.Between(1, 1000000000);
            CrashPoint = 999999999 / Convert.ToDecimal(RandomNumber);
            JoinPlayerCrashPoint = CrashPoint;
        }
        public void SetGeneralInfo()
        {
            Random rnd = new Random();
            string[] items = { "ComelyGamer", "HandyConde", "BecomingSusionec", "Aboundicsion", "GamerStabber", "DeffyOffirtes", "Membaldeffee", "Comentsisl", "2hotin", "Authorou", "Caracupti", "Diadom", "Dulenogr", "Dynaffl", "Ezonotern", "Fishpoint", "Gladney", "Grassriv", "GreeWave", "Heddaintr", "Hellight", "Icect", "Laugherso", "Maniakfi", "Nekong", "PreciseSumo", "Quoteamen", "Rappark", "Raytech", "Risoft", "RunInspire", "SunsetRoz", "Systagua", "TroikFlashy", "UpforMine", "WarSing", "WordHehe", "Xboxis" };
            NameTag = items[rnd.Next(0, items.Length)];
            UserCash = 10.00M;
        }
    }
}
