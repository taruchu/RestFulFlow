using System;
using System.Collections.Generic;

namespace SharedServices.Interfaces.Routing
{
    public interface IRoutingTable<T> : IDisposable
    {
        //NOTE: This will hold route information in key value pair. key = route, value = callback to consume message
        //If a router needs to send to another router, the callback will use the message bus bank to forward the message
        //to the other router.
        string RoutingTableGUID { get; }
        bool RegisterRoute(string route, Action<T> routeAction);
        Action<T> ResolveRoute(string route);
        bool ReleaseRoute(string route);
        IMessageBusBank<T> MessageBusBank { get; set; }
        string ExceptionMessage_RouteCannotBeNullOrEmpty { get; }
        string ExceptionMessage_RouteFormatIsIncorrect { get; }
        string ExceptionMessage_RouteActionCannotBeNull { get; }
        string ExceptionMessage_MessageBusBankCannotBeNull { get; }

        //NOTE: If the route cannot be resolved, forward the message to another router using the message bus bank 
        //The <RouterBusKeyCode> portion of the route will be used to reslove the correct message bus.
        //The <RouterBusKeyCode> will equal the RoutingTableGUID of this routing table.
    }
}
