using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.ServiceFarm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMessageInterfaces.Interfaces.ChatMessage
{
    public interface IGetNextChatMessageService : IServiceFarmServiceBase
    {
        string Get(IChatMessageEnvelope request);
        ITack Tack { get; set; }

        string ExceptionMessage_ITackCannotBeNull { get; }
    }
}
