using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.ServiceFarm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMessageInterfaces.Interfaces.ChatMessage
{
    public interface IGetNextChatMessageService : IChatMessageService
    {
        string Get(IChatMessageEnvelope request);  
        string ExceptionMessage_ITackCannotBeNull { get; }
    }
}
