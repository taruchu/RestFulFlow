using SharedServices.Interfaces.Routing;
using System;

namespace SharedServices.Models.Routing
{
    public class Route<T> : IRoute<T>
    {
        public Action<T> RegisterRouteHandler { get; set; }
        string IRoute<T>.Route { get; set; }
    }
}
