using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMessageInterfaces.Interfaces.ChatMessage
{
    public interface IGetLatestChatMessageService : IChatMessageService
    {
        /*
         * NOTE:
         * This service will poll for the latest chat message. It will poll for a configured time.
         * 
         */
        string Get(IChatMessageEnvelope request);
        double PollDurrationInMinutes { get; set; }
    }
}
