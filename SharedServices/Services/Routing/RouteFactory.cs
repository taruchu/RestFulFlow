using SharedInterfaces.Interfaces.Routing;
using SharedServices.Models.Routing;
using System;

namespace SharedServices.Services.Routing
{
    class RouteFactory<T> : IRouteFactory<T>
    {
        public IRoute<T> InstantiateIRoute()
        {
            return new Route<T>();
        }

        public Type ResolveImplementationType()
        {
            return typeof(Route<T>);
        }
    }
}
