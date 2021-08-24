using Microsoft.AspNetCore.SignalR.Client;

namespace RedEye.FakeGambling.Data
{
    public interface IHubService
    {
        HubConnection hubConnection { get; set; }

        void Dispose();
    }
}