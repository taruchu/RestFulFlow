using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.ServiceFarm;

namespace ChatMessageInterfaces.Interfaces.ChatMessage
{
    public interface IModifyChatMessageService  : IChatMessageService  
    {
        /* NOTE: Supports 
         * 
         * PUT - Edit a chat message.
         * POST - Create new chat message.
         * DELETE - Remove a chat message. 
         *
         */
        string Put(IChatMessageEnvelope request);
        string Post(IChatMessageEnvelope request);
        string Delete(IChatMessageEnvelope request);  
    }
}

