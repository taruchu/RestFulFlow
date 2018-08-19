
using SharedInterfaces.Interfaces.Marshaller;
using System;

namespace SharedInterfaces.Interfaces.Routing
{
    public interface IRoutingService<T> : IDisposable
    { 
        string RoutingServiceGUID { get; } //NOTE: (Pass-though) This should get the Routing Table's GUID
        IMessageBusReaderBank<T> MessageBusReaderBank { get; set; } 
        IMarshaller Marshaller { get; set; }
        IRoutingTable<T> RoutingTable { get; set; } //NOTE: Decouple this form the service for swap outs.
        bool RegisterRoute(IRoute<T> route);
        Action<T> ResolveRoute(string route);
        bool ReleaseRoute(IRoute<T> route);
        bool InitializeReaders(int numberOfReaders);
        string ParseMessageForRoute(T message);
        bool ForwardMessageToResolvedRoute(Action<T> resolvedRoute, T jsonMessage);
        string ExceptionMessage_RoutingTableCannotBeNull { get; }
        string ExceptionMessage_IRouteCannotBeNull { get; }
        string ExceptionMessage_RouteCannotBeNullOrEmpty { get; }
        string ExceptionMessage_MessageCannotBeNullOrEmpty { get; }
        string ExceptionMessage_ResolvedRouteCannotBeNull { get; }
        string ExceptionMessage_MessageBusReaderBankCannotBeNull { get; } 
        string ExceptionMessage_MarshallerCannotBeNull { get; }

        //TODO: Make sure to update the schema so that I can distinguish between a message
        //that is sent to the routing service directly verses a message simply being routed.
        //For direct messages the IRoute object should be one of the key/value pairs. 
        //Maybe just check if the destination route key/value pair equals the routing service's GUID ? 
    }
}
