namespace RedEye.FakeGambling.Data
{
    public interface IGameService
    {
        decimal BetAmount { get; set; }
        bool Cashit { get; set; }
        decimal CrashPoint { get; set; }
        int RandomNumber { get; set; }
        decimal UserCash { get; set; }

        void NewCrashPoint();
    }
}