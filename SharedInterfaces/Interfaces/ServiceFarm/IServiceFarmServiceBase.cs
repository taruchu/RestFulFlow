using SharedInterfaces.Interfaces.Routing;
using System;

namespace SharedInterfaces.Interfaces.ServiceFarm
{
    public interface IServiceFarmServiceBase : IDisposable
    {
        string ServiceName { get; set; }
        string ServiceGUID { get; }
        IMessageBusWriter<string> MessageBusWiter { get; set; }  
        IMessageBusReaderBank<string> MessageBusReaderBank { get; set; }
        IMessageBusBank<string> MessageBusBank { get; set; } 
        Action<string> HandleMessageFromRouter { get; set; }
        bool SendResponse(string ClientProxyGUID, string responseBody);

        string ExceptionMessage_MessageBusWriterCannotBeNull { get; }
        string ExceptionMessage_MessageBusReaderBankCannotBeNull { get; }
        string ExceptionMessage_MessageBusBankCannotBeNull { get; }
        string ExceptionMessage_MarshallerCannotBeNull { get; }
    }
}
