


using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.ServiceFarm;

namespace SharedInterfaces.Interfaces.ChatMessage
{
    public interface IChatMessageService  : IServiceFarmServiceBase //TODO: Change this to ModifyChatMessageService and create separate services for GetNewest, GetNext, GetRange etc.
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

