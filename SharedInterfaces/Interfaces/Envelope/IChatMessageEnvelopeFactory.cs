using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces.Interfaces.Envelope
{
    public interface IChatMessageEnvelopeFactory
    {
        IChatMessageEnvelope InstantiateIEnvelope();
        Type ResolveImplementationType();
    }
}
