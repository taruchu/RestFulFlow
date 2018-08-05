using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Interfaces.Proxy
{
    public interface IClientProxy
    {
        /*
         * NOTE: Represent the client within the Service Farm. 
         * It will expose a response callback that can be registered with a routing table. The router will use 
         * this callback to put response envelopes on the client proxies message bus, where the client proxy will dequeue
         * from later. Once a response is availiable on the message bus, the module that is using this client proxy can retirieve the 
         * response through a async method call that polls the message bus. 
         * 
         * 
         */
    }
}
