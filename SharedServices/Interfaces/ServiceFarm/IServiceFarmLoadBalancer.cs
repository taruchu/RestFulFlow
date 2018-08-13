 
using SharedServices.Interfaces.Routing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Interfaces.ServiceFarm
{
    public interface IServiceFarmLoadBalancer : IDisposable
    {
        /* 
         * NOTE: This interface implementation should have a singleton scope with thread safe locking.
         * This load balancer will use a load balance algorithm based on probability to find the correct router
         * to forward the message to. No global data will be used except to resolve the location of the routers.
         * It will then take the clients request envelope and salt it with a serialized IRoute instance that is composed
         * of the cients responseCallback and the destination Router's route. Once this is complete it will put the request
         * envelope on the routers message bus. 
         */

        bool CompositionRoute(); //NOTE: Call this from the constructor to New up all service farm objects, associate them to a router, and establish their composition route.
        bool SendServiceRequest(string clientProxyOrigin, string requestEnvelope);
 
        bool RegisterClientProxyMessageBus(string clientProxyGUID, IMessageBus<string> messageBus);
        bool ReleaseClientProxyMessageBus(string clientProxyGUID);
    }
}
