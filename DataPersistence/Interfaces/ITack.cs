using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public interface ITack : IDisposable
    {
        /*
         * This is a  Restful service that implements GET, POST, PUT, DELETE in order to abstract data persistence operations. The client 
         * will only see this service. Under the hood the ITack implementation will interact with the specific data persistence engine.
         * 
         * ITack will rely on the IEnvelope as the only parameter, and will have methods for mapping the IEnvelope Data Transfer Object into 
         * the specific database table objects (Database access / O.R.M.). 
         * 
         * It will use the IEnvelope.GetMyEnvelopeType() method to determine the correct persistence route, which is to say, it will use the type
         * to determine the storage mechanism. Each service has it's own envelope interface that is derived from IEnvelope. And each service will have a 
         * specific storage mechanism. 
         * 
         */
          
        IEnvelope GET(IEnvelope envelope);
        IEnvelope POST(IEnvelope envelope);
        IEnvelope PUT(IEnvelope envelope);
        IEnvelope DELETE(IEnvelope envelope);
        ISkyWatch SkyWatch { get; set; }
        IBoards Boards { get; } 
    }
}
