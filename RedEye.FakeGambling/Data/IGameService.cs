using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Data
{
    public interface IGameService
    {
        decimal BetAmount { get; set; }
        bool Cashit { get; set; }
        decimal CrashPoint { get; set; }
        int RandomNumber { get; set; }
        decimal UserCash { get; set; }
        string NameTag { get; set; }
        bool isCreatePlayer { get; set; }
        decimal JoinPlayerCrashPoint { get; set; }
        decimal LastJoinPlayerCrashPoint { get; set; }
        List<string> ChatMessages { get; set; }
        List<string> LeaderboardsHistory { get; set; }
        void NewCrashPoint();
        void SetGeneralInfo();
        bool IsRunning { get; set; }
    }
}