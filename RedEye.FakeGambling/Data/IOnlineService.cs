using System.Threading.Tasks;

namespace RedEye.FakeGambling.Data
{
    public interface IOnlineService
    {
        Task UpdateCrashAsync();
    }
}