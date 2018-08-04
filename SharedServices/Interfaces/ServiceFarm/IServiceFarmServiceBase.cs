using SharedServices.Services.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Interfaces.ServiceFarm
{
    public interface IServiceFarmServiceBase : IDisposable
    {
        MessageBusWriter<string> MessageBusWiter { get; set; }  
        MessageBusReaderBank<string> MessageBusReaderBank { get; set; }
        Action<string> HandleMessageFromRouter { get; set; }
        bool Put(string serviceUrl, string responseBody);
    }
}
