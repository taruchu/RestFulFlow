using SharedServices.Services.IOC;
using Microsoft.AspNetCore.SignalR;
using SharedInterfaces.Interfaces.ServiceFarm;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestFulFlowService.Services
{
    public class ServiceFarmHub : Hub
    { 
        private ErectDIContainer _erector { get; set; }
        private bool _isConnected { get; set; }
        public ServiceFarmHub()
        {
            _erector = new ErectDIContainer();
            _isConnected = false;
        }

        public async Task GetLatestChatMessages(string json)
        {
            string response = string.Empty; 
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            SharedInterfaces.Interfaces.Proxy.IClientProxy clientProxy = _erector.Container.Resolve<SharedInterfaces.Interfaces.Proxy.IClientProxy>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            serviceFarmLoadBalancer.RegisterClientProxyMessageBus(clientProxy);

            while(_isConnected)
            {
                if (serviceFarmLoadBalancer.SendServiceRequest(clientProxy.ServiceGUID, json))
                {
                    do
                    { 
                        response = clientProxy.PollMessageBus(cancellationTokenSource);
                        await Clients.Caller.SendAsync("ReceiveLatestChatMessage", response);
                    }
                    while (String.IsNullOrEmpty(response) == false);
                } 
            }            
        }

        public override async Task OnConnectedAsync()
        { 
            await base.OnConnectedAsync();
            _isConnected = true;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        { 
            await base.OnDisconnectedAsync(exception);
            _isConnected = false;
        }
    }
}
