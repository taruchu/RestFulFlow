using SharedInterfaces.Interfaces.Envelope;
using System;

namespace SharedInterfaces.Models.Envelope
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
