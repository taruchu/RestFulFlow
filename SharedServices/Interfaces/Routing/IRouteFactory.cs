using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Interfaces.Routing
{
    public interface IRouteFactory<T>
    {
        IRoute<T> InstantiateIRoute();
        Type ResolveImplementationType();
    }
}
