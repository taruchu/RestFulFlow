using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Interfaces.ServiceFarm
{
    public interface IServiceFarmLoadBalancer : IDisposable
    {
        //NOTE: This interface implementation should have a singleton scope with thread safe locking

        bool CompositionRoute(); //NOTE: New up all objects, associate them to a router, and establish their composition route.
        IReadOnlyCollection<IServiceFarmServiceBase> ServiceList { get; }  
        bool SendServiceRequest(string requestEnvelope, Action<string> responseCallback = null);
    }
}
