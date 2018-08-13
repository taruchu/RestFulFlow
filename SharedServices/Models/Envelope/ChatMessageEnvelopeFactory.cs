using SharedServices.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Models.Envelope
{
    public class ChatMessageEnvelopeFactory : IChatMessageEnvelopeFactory
    {
        public IChatMessageEnvelope InstantiateIEnvelope()
        {
            return new ChatMessageEnvelope();
        }

        public Type ResolveImplementationType()
        {
            return typeof(ChatMessageEnvelope);
        }
    }
}
