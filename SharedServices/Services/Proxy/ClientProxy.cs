﻿using SharedServices.Interfaces.Marshaller;
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
        public IMessageBusBank<string> MessageBusBank { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMarshaller Marshaller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<string> HandleMessageFromRouter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ExceptionMessage_MessageBusWriterCannotBeNull => throw new NotImplementedException();

        public string ExceptionMessage_MessageBusReaderBankCannotBeNull => throw new NotImplementedException();

        public string ExceptionMessage_MessageBusBankCannotBeNull => throw new NotImplementedException();

        public string ExceptionMessage_MarshallerCannotBeNull => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<string> PollMessageBus()
        {
            throw new NotImplementedException();
        }

        public bool SendResponse(string clientProxyGUID, string responseBody)
        {
            throw new NotImplementedException();
        }
    }
}
