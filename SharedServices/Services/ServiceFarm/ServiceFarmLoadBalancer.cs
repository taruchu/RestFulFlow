using DataPersistence.Interfaces;
using log4net;
using log4net.Config;
using SharedInterfaces.Interfaces.Envelope;
using SharedUtilities.Interfaces.Marshall;
using SharedInterfaces.Interfaces.Routing;
using SharedInterfaces.Interfaces.ServiceFarm;
using SharedServices.Models.Constants;
using SharedServices.Services.IOC;
using System;
using System.Collections.Generic;
using ChatMessageInterfaces.Interfaces.ChatMessage;
using SharedInterfaces.Interfaces.Proxy;
using System.Reflection;

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
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new System.IO.FileInfo(ConfigurationConstants.FileName_log4NetConfiguration));
            _thisLock = new object();
            _isDisposed = false;
            _serviceList = new List<IDisposable>();            
            _erector = new ErectDIContainer();
            _marshaller = _erector.Container.Resolve<IMarshaller>();
            CompositionRoute();
        }

        public void RoutingServiceCompositionRoute(IRoutingService<string> routingService, ISkyWatch skyWatch)
        {
            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();
            messageBus.JsonSchema =
            (message) =>
            {
                IEnvelope envelope = _marshaller.UnMarshall<IEnvelope>(message);
                string serviceName = envelope.ServiceRoute.Split('.')[1];
                if (serviceName == ChatServiceNames.ModifyChatMessageService || serviceName == ChatServiceNames.GetNextChatMessageService)
                    return _erector.Container.Resolve<IChatMessageEnvelope>().GetMyJSONSchema();
                else
                    return String.Empty;
            };

            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            messageBusReaderBank.SpecifyTheMessageBus(messageBus);

            IRoutingTable<string> routingTable = _erector.Container.Resolve<IRoutingTable<string>>();
            routingTable.MessageBusBank = _messageBusBankRouters;

            routingService.RoutingTable = routingTable;
            routingService.MessageBusReaderBank = messageBusReaderBank;
            //NOTE: Set up two readers
            routingService.InitializeReaders(2);

            _messageBusBankRouters.RegisterMessageBus(routingService.RoutingServiceGUID, messageBus);
            _serviceList.Add(routingService);
        }

        public void ChatMessageServiceCompositionRoute(IChatMessageService chatMessageService, IRoutingService<string> routingService, ISkyWatch skyWatch)
        {
            ITack tackModifyChatMessageServiceA = _erector.Container.Resolve<ITack>();
            tackModifyChatMessageServiceA.SkyWatch = skyWatch;
            chatMessageService.Tack = tackModifyChatMessageServiceA;
        }

        public void ServiceFarmServiceCompositionRoute(IServiceFarmServiceBase serviceFarmServiceBase, IRoutingService<string> routingService, string ChatServiceName)
        {
            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();
            messageBus.JsonSchema =
            (message) =>
            {
                IEnvelope envelope = _marshaller.UnMarshall<IEnvelope>(message);
                string serviceName = envelope.ServiceRoute.Split('.')[1];
                if (serviceName == ChatServiceNames.ModifyChatMessageService || serviceName == ChatServiceNames.GetNextChatMessageService)
                    return _erector.Container.Resolve<IChatMessageEnvelope>().GetMyJSONSchema();
                else
                    return String.Empty;
            };

            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            messageBusReaderBank.SpecifyTheMessageBus(messageBus);

            IMessageBusWriter<string> messageBusWriter = _erector.Container.Resolve<IMessageBusWriter<string>>();
            messageBusWriter.SpecifyTheMessageBus(messageBus);
             
            //TODO Refactor reader setup so that this behaves like the router and uses an Initializer method
            messageBusReaderBank.AddAnotherReader(serviceFarmServiceBase.ProcessMessage);
            messageBusReaderBank.AddAnotherReader(serviceFarmServiceBase.ProcessMessage);
            serviceFarmServiceBase.MessageBusReaderBank = messageBusReaderBank;

            serviceFarmServiceBase.MessageBusWiter = messageBusWriter;
            serviceFarmServiceBase.MessageBusBank = _messageBusBankServices; 

            IRoute<string> route = _erector.Container.Resolve<IRoute<string>>();
            route.Route = String.Format("{0}.{1}", routingService.RoutingServiceGUID, ChatServiceName);
            route.RegisterRouteHandler = serviceFarmServiceBase.HandleMessageFromRouter;
            routingService.RegisterRoute(route);

            _messageBusBankServices.RegisterMessageBus(serviceFarmServiceBase.ServiceGUID, messageBus);
            _serviceList.Add(serviceFarmServiceBase);
        }

        public bool CompositionRoute()
        {
            lock (_thisLock)
            {
                try
                {
                    //NOTE: Set up message bus banks
                    _messageBusBankRouters = _erector.Container.Resolve<IMessageBusBank<string>>();
                    _messageBusBankServices = _erector.Container.Resolve<IMessageBusBank<string>>();

                    //**** Set up Network A ****/
                    //NOTE: Set up router A 
                        IRoutingService<string> routingServiceA = _erector.Container.Resolve<IRoutingService<string>>();
                        ISkyWatch skyWatchA = _erector.Container.Resolve<ISkyWatch>();

                        //NOTE: Set up the ModifyChatMessageService A 
                        IModifyChatMessageService modifyChatMessageServiceA = _erector.Container.Resolve<IModifyChatMessageService>();
                        ServiceFarmServiceCompositionRoute(modifyChatMessageServiceA, routingServiceA, ChatServiceNames.ModifyChatMessageService);
                        ChatMessageServiceCompositionRoute(modifyChatMessageServiceA, routingServiceA, skyWatchA);

                        //NOTE: Set up the GetNextChatMessageService A 
                        IGetNextChatMessageService getNextChatMessageServiceA = _erector.Container.Resolve<IGetNextChatMessageService>();
                        ServiceFarmServiceCompositionRoute(getNextChatMessageServiceA, routingServiceA, ChatServiceNames.GetNextChatMessageService);
                        ChatMessageServiceCompositionRoute(getNextChatMessageServiceA, routingServiceA, skyWatchA); 

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
                    if(serviceName == ChatServiceNames.GetNextChatMessageService || serviceName == ChatServiceNames.ModifyChatMessageService)
                    {
                        IChatMessageEnvelope chatMessageEnvelope = _marshaller.UnMarshall<IChatMessageEnvelope>(requestEnvelope);

                        chatMessageEnvelope.ServiceRoute = String.Format("{0}.{1}", routerBusKeyCode, serviceName); //NOTE: Salt the route by adding the router.                        
                        chatMessageEnvelope.ClientProxyGUID = ClientProxyGUID;

                        string saltedRequest = _marshaller.MarshallPayloadJSON(chatMessageEnvelope);
                        _messageBusBankRouters.ResolveMessageBus(routerBusKeyCode).SendMessage(saltedRequest);
                        return true;
                    }
                    return false;
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

        public bool RegisterClientProxyMessageBus(IClientProxy clientProxy)
        {
            lock (_thisLock)
            { 
                try
                {
                    IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();
                    messageBus.SkipValidation = true;

                    IMessageBusReaderBank<string>  messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
                    messageBusReaderBank.SpecifyTheMessageBus(messageBus);

                    IMessageBusWriter<string> messageBusWriter = _erector.Container.Resolve<IMessageBusWriter<string>>();
                    messageBusWriter.SpecifyTheMessageBus(messageBus);

                    clientProxy.MessageBusReaderBank = messageBusReaderBank;
                    clientProxy.MessageBusWiter = messageBusWriter;

                    return _messageBusBankServices.RegisterMessageBus(clientProxy.ServiceGUID, messageBus);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                } 
            }
        }

        public bool ReleaseClientProxyMessageBus(IClientProxy clientProxy)
        {
            lock (_thisLock)
            { 
                try
                {
                    return _messageBusBankServices.ReleaseMessageBus(clientProxy.ServiceGUID);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                } 
            }
        }
    }
}
