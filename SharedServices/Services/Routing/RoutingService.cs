using SharedServices.Interfaces.Envelope;
using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.Routing;
using SharedServices.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Services.Routing
{
    public class RoutingService<T> : IRoutingService<T>
    {
        public string RoutingServiceGUID
        {
            get
            {
                return (RoutingTable != null) ? RoutingTable.RoutingTableGUID : String.Empty; 
            }
        }
        public IMessageBusReaderBank<T> MessageBusReaderBank { get; set; } 
        public IRoutingTable<T> RoutingTable { get; set; }
        public IMarshaller Marshaller { get; set; }
        public string ExceptionMessage_RoutingTableCannotBeNull
        {
            get
            {
                return "RoutingService<T> - RoutingTable cannot be null.";
            }
        }
        public string ExceptionMessage_IRouteCannotBeNull
        {
            get
            {
                return "RoutingService<T> - IRoute cannot be null.";
            }
        }
        public string ExceptionMessage_RouteCannotBeNullOrEmpty
        {
            get
            {
                return "RoutingService<T> - Route cannot be null or empty.";
            }
        }
        public string ExceptionMessage_MessageCannotBeNullOrEmpty
        {
            get
            {
                return "RoutingService<T> - Message cannot be null or empty.";
            }
        }
        public string ExceptionMessage_ResolvedRouteCannotBeNull
        {
            get
            {
                return "RoutingService<T> - ResolvedRoute cannot be null.";
            }
        }
        public string ExceptionMessage_MessageBusReaderBankCannotBeNull
        {
            get
            {
                return "RoutingService<T> - MessageBusReaderBank cannot be null.";
            }
        } 
        public string ExceptionMessage_MarshallerCannotBeNull
        {
            get
            {
                return "RoutingService<T> - Marshaller cannot be null.";
            }
        }
        private bool _isDisposed { get; set; }
        private object _thisLock { get; set; }

        public RoutingService()
        {
            _isDisposed = false;
            _thisLock = new object();
        }

        public void Dispose()
        {
            if (_isDisposed == false)
            {
                if (RoutingTable != null)
                    RoutingTable.Dispose();
                if (MessageBusReaderBank != null)
                    MessageBusReaderBank.Dispose(); 

                _isDisposed = true;
            }
        }

        public bool ForwardMessageToResolvedRoute(Action<T> resolvedRoute, T jsonMessage)
        {
            lock(_thisLock)
            {
                try
                {
                    if (resolvedRoute == null)
                        throw new InvalidOperationException(ExceptionMessage_ResolvedRouteCannotBeNull);
                    else if (jsonMessage == null)
                        throw new InvalidOperationException(ExceptionMessage_MessageCannotBeNullOrEmpty);
                    else
                    {
                        resolvedRoute(jsonMessage);
                        return true;
                    }
                }
                catch(InvalidOperationException ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public string ParseMessageForRoute(T message)
        {
            lock(_thisLock)
            {
                try
                {
                    string jsonMessage = (string)Convert.ChangeType(message, typeof(string));
                    if (String.IsNullOrEmpty(jsonMessage))
                        throw new InvalidOperationException(ExceptionMessage_MessageCannotBeNullOrEmpty);
                    else if (Marshaller == null)
                        throw new InvalidOperationException(ExceptionMessage_MarshallerCannotBeNull);
                    else
                    {                        
                        IEnvelope envelope = Marshaller.UnMarshall(jsonMessage);
                        string destinationRoute = envelope.Header_KeyValues[JSONSchemas.DestinationRoute];
                        return destinationRoute;
                    }
                }
                catch(InvalidOperationException ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }
        
        public bool RegisterRoute(IRoute<T> route)
        {
            lock(_thisLock)
            {
                try
                {
                    if (route == null)
                        throw new InvalidOperationException(ExceptionMessage_IRouteCannotBeNull);
                    else if (RoutingTable == null)
                        throw new InvalidOperationException(ExceptionMessage_RoutingTableCannotBeNull);
                    else
                    {
                        return RoutingTable.RegisterRoute(route.Route, route.RegisterRouteHandler);
                    }
                }
                catch(InvalidOperationException ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public Action<T> ResolveRoute(string route)
        {
            lock(_thisLock)
            {
                try
                {
                    if (String.IsNullOrEmpty(route))
                        throw new InvalidOperationException(ExceptionMessage_RouteCannotBeNullOrEmpty);
                    else if (RoutingTable == null)
                        throw new InvalidOperationException(ExceptionMessage_RoutingTableCannotBeNull);
                    else
                    {
                        return RoutingTable.ResolveRoute(route);
                    }
                }
                catch(InvalidOperationException ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public bool ReleaseRoute(IRoute<T> route)
        {
            lock (_thisLock)
            {
                try
                {
                    if (route == null)
                        throw new InvalidOperationException(ExceptionMessage_IRouteCannotBeNull);
                    else if (RoutingTable == null)
                        throw new InvalidOperationException(ExceptionMessage_RoutingTableCannotBeNull);
                    else
                    {
                        return RoutingTable.ReleaseRoute(route.Route);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public void MessageBusReaderCallback(T message)
        {
            lock (_thisLock)
            {
                string destinationRoute = ParseMessageForRoute(message);
                if (destinationRoute == RoutingServiceGUID)//NOTE: This message is for the router.
                {
                    if (Marshaller == null)
                        throw new InvalidOperationException(ExceptionMessage_MarshallerCannotBeNull);
                    else
                    {
                        //TODO: Support for marshalling another object type into IEnvelope ? Overloads ??
                        string jsonMessage = (string)Convert.ChangeType(message, typeof(string));
                        IEnvelope envelope = Marshaller.UnMarshall(jsonMessage);
                        string routingServiceCommand = envelope.Payload_KeyValues[JSONSchemas.RoutingServiceCommand];
                        string routingServiceRoute = envelope.Payload_KeyValues[JSONSchemas.Route];
                        IRoute<T> route = Marshaller.UnMarshall<IRoute<T>>(routingServiceRoute);
                        if (routingServiceCommand == JSONSchemas.RoutingServiceCommandRegister)
                            RegisterRoute(route);
                        else if (routingServiceCommand == JSONSchemas.RoutingServiceCommandRelease)
                            ReleaseRoute(route);
                    }
                }
                else
                {
                    Action<T> reslovedRoute = ResolveRoute(destinationRoute);
                    ForwardMessageToResolvedRoute(reslovedRoute, message);
                }
            }
        }

        public bool InitializeReaders(int numberOfReaders)
        {
            lock(_thisLock)
            {
                try
                {
                    if (MessageBusReaderBank == null)
                        throw new InvalidOperationException(ExceptionMessage_MessageBusReaderBankCannotBeNull);
                    else
                    {
                        for(int readerIndex = 0; readerIndex < numberOfReaders; readerIndex++)
                        {
                            //NOTE: Must test this within the MessageBusReaderBank Unit Test.
                            MessageBusReaderBank.AddAnotherReader(MessageBusReaderCallback);
                        }
                        return true;
                    }
                }
                catch(InvalidOperationException ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        } 
    }
}
