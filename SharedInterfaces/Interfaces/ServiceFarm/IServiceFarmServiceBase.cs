using SharedInterfaces.Interfaces.Marshaller;
using SharedInterfaces.Interfaces.Routing; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces.Interfaces.ServiceFarm
{
    public interface IServiceFarmServiceBase : IDisposable
    {
        string ServiceName { get; set; }
        string ServiceGUID { get; }
        IMessageBusWriter<string> MessageBusWiter { get; set; }  
        IMessageBusReaderBank<string> MessageBusReaderBank { get; set; }
        IMessageBusBank<string> MessageBusBank { get; set; }
        IMarshaller Marshaller { get; set; }
        Action<string> HandleMessageFromRouter { get; set; }
        bool SendResponse(string clientProxyGUID, string responseBody);

        string ExceptionMessage_MessageBusWriterCannotBeNull { get; }
        string ExceptionMessage_MessageBusReaderBankCannotBeNull { get; }
        string ExceptionMessage_MessageBusBankCannotBeNull { get; }
        string ExceptionMessage_MarshallerCannotBeNull { get; }
    }
}
