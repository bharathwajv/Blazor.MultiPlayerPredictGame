using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Data
{
    public class HubService : IDisposable
    {
        public HubConnection hubConnection { get; set; }

        private readonly NavigationManager _navigationManager;
        //public HubService(NavigationManager navigationManager)
        //{
        //    _navigationManager = navigationManager;
        //}
        //public void OnInit()
        //{
        //    hubConnection = new HubConnectionBuilder()
        //            .WithUrl(_navigationManager.ToAbsoluteUri("/gameHub"))
        //            .Build();
        //}

        public void Dispose()
        {
            if (hubConnection is not null)
            {
                hubConnection.DisposeAsync();
            }
        }
    }
}
