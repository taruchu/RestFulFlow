using System;
using System.Collections.Generic;
using System.Text;

namespace SharedInterfaces.Interfaces.Envelope
{
    public interface IChatMessageQueryRepository
    {
        IChatMessageEnvelope GetNextChatMessage(IChatMessageEnvelope chatMessageEnvelope); 
        IChatMessageEnvelope GetChatMessageByID(IChatMessageEnvelope chatMessageEnvelope); 
    }
}
