using System;

namespace SharedInterfaces.Interfaces.Envelope
{
    public interface IEnvelopeFactory
    {
        IEnvelope InstantiateIEnvelope();
        Type ResolveImplementationType(); 
    }
}
