using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.Proxy;
using SharedServices.Interfaces.Routing;
using SharedServices.Interfaces.ServiceFarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Services.Proxy
{
    public class ClientProxy : IClientProxy
    {
        public string ServiceName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ServiceGUID => throw new NotImplementedException();

        public IMessageBusWriter<string> MessageBusWiter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMessageBusReaderBank<string> MessageBusReaderBank { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMarshaller Marshaller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<string> HandleMessageFromRouter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<string> PollMessageBus()
        {
            throw new NotImplementedException();
        }

        public bool Post(string serviceUrl, string responseBody)
        {
            throw new NotImplementedException();
        }
    }
}
