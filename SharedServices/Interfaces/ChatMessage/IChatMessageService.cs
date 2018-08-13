


using SharedServices.Interfaces.Envelope;
using SharedServices.Interfaces.ServiceFarm;

namespace SharedServices.Interfaces.ChatMessage
{
    public interface IChatMessageService  : IServiceFarmServiceBase
    {
        /* NOTE: Supports 
         * GET - Get the newest chat message for a channel
         * PUT - Edit a chat message.
         * POST - Create new chat message.
         * DELETE - Remove a chat message. 
         *
         */
        string GetNewest(IChatMessageEnvelope request);
        string Put(IChatMessageEnvelope request);
        string Post(IChatMessageEnvelope request);
        string Delete(IChatMessageEnvelope request);
    }
}

