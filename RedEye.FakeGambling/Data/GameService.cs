using System;

namespace RedEye.FakeGambling.Data
{
    public class GameService : IGameService
    {
        public bool Cashit { get; set; } = false;
        public decimal UserCash { get; set; } = 10.00M;
        public decimal BetAmount { get; set; } = 0;
        public int RandomNumber { get; set; }
        public decimal CrashPoint { get; set; }

        public void NewCrashPoint()
        {
            // Use RNGCryptoService function to create a very large random number, making the multiplier small.
            RandomNumber = Values.Between(1, 1000000000);
            CrashPoint = 999999999 / Convert.ToDecimal(RandomNumber);
        }
    }
}
