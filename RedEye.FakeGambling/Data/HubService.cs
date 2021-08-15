using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Data
{
    public class HubService : IAsyncDisposable
    {
        public HubConnection hubConnection { get; set; }

        private readonly NavigationManager _navigationManager;
        public HubService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }
        public void OnInit()
        {
            hubConnection = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri("/gamehub"))
                    .Build();
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }
}
