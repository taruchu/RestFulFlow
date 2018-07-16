using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Interfaces.ServiceFarm
{
    public interface IServiceFarm : IDisposable
    {
        bool CompositionRoute(); //NOTE: New up all objects, associate them to a router, and establish their composition route.
        List<IServiceFarmServiceBase> ServiceList { get; set; }

    }
}
