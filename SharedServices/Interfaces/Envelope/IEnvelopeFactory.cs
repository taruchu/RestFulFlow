using System;

namespace SharedServices.Interfaces.Envelope
{
    public interface IEnvelopeFactory
    {
        IEnvelope InstantiateIEnvelope();
        Type ResolveImplementationType(); 
    }
}
