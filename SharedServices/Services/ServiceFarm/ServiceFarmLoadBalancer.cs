using DataPersistence.Interfaces;
using log4net;
using log4net.Config;
using SharedInterfaces.Interfaces.ChatMessage;
using SharedInterfaces.Interfaces.Envelope;
using SharedUtilities.Interfaces.Marshall;
using SharedInterfaces.Interfaces.Routing;
using SharedInterfaces.Interfaces.ServiceFarm;
using SharedServices.Models.Constants;
using SharedServices.Services.IOC;
using System;
using System.Collections.Generic;

namespace SharedServices.Services.ServiceFarm
{
    public class ServiceFarmLoadBalancer : IServiceFarmLoadBalancer
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ServiceFarmLoadBalancer));
        private List<IDisposable> _serviceList { get; set; }
        private ErectDIContainer _erector { get; set; }
        private IMessageBusBank<string> _messageBusBankRouters { get; set; }
        private IMessageBusBank<string> _messageBusBankServices { get; set; } 
        private bool _isDisposed { get; set; }
        private object _thisLock { get; set; }
        private IMarshaller _marshaller { get; set; } 

        public ServiceFarmLoadBalancer()
        {
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(ConfigurationConstants.FileName_log4NetConfiguration));
            _thisLock = new object();
            _isDisposed = false;
            _serviceList = new List<IDisposable>();            
            _erector = new ErectDIContainer();
            _marshaller = _erector.Container.Resolve<IMarshaller>();
            CompositionRoute();
        }

        public bool CompositionRoute()
        {
            lock (_thisLock)
            {
                try
                {
                    //NOTE: Set up message bus bank
                    _messageBusBankRouters = _erector.Container.Resolve<IMessageBusBank<string>>();
                    _messageBusBankServices = _erector.Container.Resolve<IMessageBusBank<string>>();

                    //**** Set up Network A ****/
                        //NOTE: Set up router A
                        IMessageBus<string> messageBusRouterA = _erector.Container.Resolve<IMessageBus<string>>();
                        messageBusRouterA.JsonSchema =
                        (message) =>
                        {
                            string serviceName = _marshaller.UnMarshall<IEnvelope>(message).ServiceRoute.Split('.')[1];
                            if (serviceName == ChatServiceNames.ChatMessageService)
                                return _erector.Container.Resolve<IChatMessageEnvelope>().GetMyJSONSchema();
                            else
                                return String.Empty;
                        };

                        IMessageBusReaderBank<string> messageBusReaderBankRouterA = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
                        messageBusReaderBankRouterA.SpecifyTheMessageBus(messageBusRouterA);

                        IRoutingTable<string> routingTableRouterA = _erector.Container.Resolve<IRoutingTable<string>>();
                        routingTableRouterA.MessageBusBank = _messageBusBankRouters;

                        IRoutingService<string> routingServiceRouterA = _erector.Container.Resolve<IRoutingService<string>>();
                        routingServiceRouterA.Marshaller = _erector.Container.Resolve<IMarshaller>();
                        routingServiceRouterA.RoutingTable = routingTableRouterA;
                        routingServiceRouterA.MessageBusReaderBank = messageBusReaderBankRouterA;

                        _messageBusBankRouters.RegisterMessageBus(routingServiceRouterA.RoutingServiceGUID, messageBusRouterA);
                        _serviceList.Add(routingServiceRouterA);

                        //NOTE: Set up the ChatMessageService A
                        IMessageBus<string> messageBusChatMessageServiceA = _erector.Container.Resolve<IMessageBus<string>>();
                        messageBusChatMessageServiceA.JsonSchema =
                        (message) =>
                        {
                            return _erector.Container.Resolve<IChatMessageEnvelope>().GetMyJSONSchema();
                        };

                        IMessageBusReaderBank<string> messageBusReaderBankChangeMessageServiceA = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
                        messageBusReaderBankChangeMessageServiceA.SpecifyTheMessageBus(messageBusChatMessageServiceA);

                        IMessageBusWriter<string> messageBusWriterChatMessageServiceA = _erector.Container.Resolve<IMessageBusWriter<string>>();
                        messageBusWriterChatMessageServiceA.SpecifyTheMessageBus(messageBusChatMessageServiceA);

                        IChatMessageService chatMessageServiceA = _erector.Container.Resolve<IChatMessageService>();
                        chatMessageServiceA.MessageBusReaderBank = messageBusReaderBankChangeMessageServiceA;
                        chatMessageServiceA.MessageBusWiter = messageBusWriterChatMessageServiceA;
                        chatMessageServiceA.Marshaller = _erector.Container.Resolve<IMarshaller>();
                        chatMessageServiceA.MessageBusBank = _messageBusBankServices;

                        IRoute<string> routeChatMessageServiceA = _erector.Container.Resolve<IRoute<string>>();
                        routeChatMessageServiceA.Route = String.Format("{0}.{1}", routingServiceRouterA.RoutingServiceGUID, ChatServiceNames.ChatMessageService);
                        routeChatMessageServiceA.RegisterRouteHandler = chatMessageServiceA.HandleMessageFromRouter;
                        routingServiceRouterA.RegisterRoute(routeChatMessageServiceA);

                        _messageBusBankServices.RegisterMessageBus(chatMessageServiceA.ServiceGUID, messageBusChatMessageServiceA);
                        _serviceList.Add(chatMessageServiceA);

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(String.Format("CompositionRoute() - Exception: \n{0}\n{1} \n\n InnerException: \n{2}\n{3}\n\n", ex.Message, ex.StackTrace, ex.InnerException.Message, ex.InnerException.StackTrace));
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public void Dispose()
        {
            lock (_thisLock)
            {
                try
                {
                    if (_isDisposed == false)
                    {
                        foreach (IDisposable service in _serviceList)
                            service.Dispose();

                        _messageBusBankServices.Dispose();
                        _messageBusBankRouters.Dispose();

                        _isDisposed = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public bool SendServiceRequest(string ClientProxyGUID,  string requestEnvelope)
        {
            lock(_thisLock)
            {
                try
                {
                    IEnvelope envelope = _marshaller.UnMarshall(requestEnvelope); 
                    int routerIndex = LoadBalanceAlgorithm(_messageBusBankRouters.GetBusKeyCodes().Count);
                    string routerBusKeyCode = _messageBusBankRouters.GetBusKeyCodes()[routerIndex];
                    string serviceName = envelope.ServiceRoute;
                    envelope.ServiceRoute = String.Format("{0}.{1}", routerBusKeyCode, serviceName); //NOTE: Salt the route by adding the router.                        
                     
                    string saltedRequest = _marshaller.MarshallPayloadJSON(envelope);
                    _messageBusBankRouters.ResolveMessageBus(routerBusKeyCode).SendMessage(saltedRequest);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(String.Format("CompositionRoute() - Exception: \n{0}\n{1} \n\n InnerException: \n{2}\n{3}\n\n", ex.Message, ex.StackTrace, ex.InnerException.Message, ex.InnerException.StackTrace));
                    throw new ApplicationException(ex.Message, ex);
                } 
            }
        }

        private int LoadBalanceAlgorithm(int numberOfItemsToLoadBalance)
        {
            //TODO: Return the hash index for an item using a probablitiy algorithm
            return 0;
        }

        public bool RegisterClientProxyMessageBus(string clientProxyGUID, IMessageBus<string> messageBus)
        {
            lock (_thisLock)
            {
                //TODO: Error messages
                try
                {
                    return _messageBusBankServices.RegisterMessageBus(clientProxyGUID, messageBus);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                } 
            }
        }

        public bool ReleaseClientProxyMessageBus(string clientProxyGUID)
        {
            lock (_thisLock)
            {
                //TODO: Error messages
                try
                {
                    return _messageBusBankServices.ReleaseMessageBus(clientProxyGUID);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                } 
            }
        }
    }
}
