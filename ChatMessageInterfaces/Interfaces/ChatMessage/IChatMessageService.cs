using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.ServiceFarm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMessageInterfaces.Interfaces.ChatMessage
{
    public interface IChatMessageService : IServiceFarmServiceBase
    {
        ITack Tack { get; set; }
        string ExceptionMessage_ITackCannotBeNull { get; }
    }
}
