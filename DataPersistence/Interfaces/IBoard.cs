using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public interface IBoard : IDisposable
    {
        bool Connect();
        IEnvelope GET(IEnvelope envelope);
        List<IEnvelope> GETList(IEnvelope envelope);
        IEnvelope PUT(IEnvelope envelope);
        IEnvelope POST(IEnvelope envelope);
        IEnvelope DELETE(IEnvelope envelope); 
    }
}
