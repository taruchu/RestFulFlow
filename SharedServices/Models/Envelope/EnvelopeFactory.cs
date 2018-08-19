using System;
using SharedInterfaces.Interfaces.Envelope;

namespace SharedServices.Models.EnvelopeModel
{
    public class EnvelopeFactory : IEnvelopeFactory
    {
        public IEnvelope InstantiateIEnvelope()
        {
            return new EnvelopeModel();
        }

        public Type ResolveImplementationType()
        {
            return typeof(EnvelopeModel);
        }
    }
}
