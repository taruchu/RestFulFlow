using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.ServiceFarm;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedInterfaces.Interfaces.ChatMessage
{
    public interface IGetNextChatMessageService : IServiceFarmServiceBase
    {
        string Get(IChatMessageEnvelope request);
    }
}
