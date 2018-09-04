﻿using DataPersistence.Interfaces;
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
                            IMessageBus<string> messageBusRouterA = _erector.Container.Resolve<IMessageBus<string>>();
                            messageBusRouterA.JsonSchema =
                            (message) =>
                            {
                                IEnvelope envelope = _marshaller.UnMarshall<IEnvelope>(message);
                                string serviceName = envelope.ServiceRoute.Split('.')[1];
                                if (serviceName == ChatServiceNames.ModifyChatMessageService || serviceName == ChatServiceNames.GetNextChatMessageService)
                                    return _erector.Container.Resolve<IChatMessageEnvelope>().GetMyJSONSchema();
                                else
                                    return String.Empty;
                            };

                            IMessageBusReaderBank<string> messageBusReaderBankRouterA = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
                            messageBusReaderBankRouterA.SpecifyTheMessageBus(messageBusRouterA);

                            IRoutingTable<string> routingTableRouterA = _erector.Container.Resolve<IRoutingTable<string>>();
                            routingTableRouterA.MessageBusBank = _messageBusBankRouters;

                            IRoutingService<string> routingServiceRouterA = _erector.Container.Resolve<IRoutingService<string>>(); 
                            routingServiceRouterA.RoutingTable = routingTableRouterA; 
                            routingServiceRouterA.MessageBusReaderBank = messageBusReaderBankRouterA;
                            //NOTE: Set up two readers
                            routingServiceRouterA.InitializeReaders(2);

                            _messageBusBankRouters.RegisterMessageBus(routingServiceRouterA.RoutingServiceGUID, messageBusRouterA);
                            _serviceList.Add(routingServiceRouterA);

                            ISkyWatch skyWatchA = _erector.Container.Resolve<ISkyWatch>();

                        //NOTE: Set up the ModifyChatMessageService A 
                            IMessageBus<string> messageBusModifyChatMessageServiceA = _erector.Container.Resolve<IMessageBus<string>>();
                            messageBusModifyChatMessageServiceA.JsonSchema =
                            (message) =>
                            {
                                return _erector.Container.Resolve<IChatMessageEnvelope>().GetMyJSONSchema(); //NOTE: Require this schema
                            };

                            IMessageBusReaderBank<string> messageBusReaderBankModifyChangeMessageServiceA = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
                            messageBusReaderBankModifyChangeMessageServiceA.SpecifyTheMessageBus(messageBusModifyChatMessageServiceA);

                            IMessageBusWriter<string> messageBusWriterModifyChatMessageServiceA = _erector.Container.Resolve<IMessageBusWriter<string>>();
                            messageBusWriterModifyChatMessageServiceA.SpecifyTheMessageBus(messageBusModifyChatMessageServiceA); 

                            IModifyChatMessageService modifyChatMessageServiceA = _erector.Container.Resolve<IModifyChatMessageService>();
                            //TODO Refactor reader setup so that this behaves like the router and uses an Initializer method
                            messageBusReaderBankModifyChangeMessageServiceA.AddAnotherReader(modifyChatMessageServiceA.ProcessMessage);
                            messageBusReaderBankModifyChangeMessageServiceA.AddAnotherReader(modifyChatMessageServiceA.ProcessMessage);
                            modifyChatMessageServiceA.MessageBusReaderBank = messageBusReaderBankModifyChangeMessageServiceA;
                            
                            modifyChatMessageServiceA.MessageBusWiter = messageBusWriterModifyChatMessageServiceA;
                            modifyChatMessageServiceA.MessageBusBank = _messageBusBankServices;
                            ITack tackModifyChatMessageServiceA = _erector.Container.Resolve<ITack>();
                            tackModifyChatMessageServiceA.SkyWatch = skyWatchA; 
                            modifyChatMessageServiceA.Tack = tackModifyChatMessageServiceA;

                            IRoute<string> routeModifyChatMessageServiceA = _erector.Container.Resolve<IRoute<string>>();
                            routeModifyChatMessageServiceA.Route = String.Format("{0}.{1}", routingServiceRouterA.RoutingServiceGUID, ChatServiceNames.ModifyChatMessageService);
                            routeModifyChatMessageServiceA.RegisterRouteHandler = modifyChatMessageServiceA.HandleMessageFromRouter;
                            routingServiceRouterA.RegisterRoute(routeModifyChatMessageServiceA);

                            _messageBusBankServices.RegisterMessageBus(modifyChatMessageServiceA.ServiceGUID, messageBusModifyChatMessageServiceA);
                            _serviceList.Add(modifyChatMessageServiceA);

                        //NOTE: Set up the GetNextChatMessageService A
                            IMessageBus<string> messageBusGetNextChatMessageServiceA = _erector.Container.Resolve<IMessageBus<string>>();
                            messageBusGetNextChatMessageServiceA.JsonSchema =
                            (message) =>
                            {
                                return _erector.Container.Resolve<IChatMessageEnvelope>().GetMyJSONSchema(); //NOTE: Require this schema
                                    };

                            IMessageBusReaderBank<string> messageBusReaderBankGetNextChangeMessageServiceA = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
                            messageBusReaderBankGetNextChangeMessageServiceA.SpecifyTheMessageBus(messageBusGetNextChatMessageServiceA);

                            IMessageBusWriter<string> messageBusWriterGetNextChatMessageServiceA = _erector.Container.Resolve<IMessageBusWriter<string>>();
                            messageBusWriterGetNextChatMessageServiceA.SpecifyTheMessageBus(messageBusGetNextChatMessageServiceA);

                            IGetNextChatMessageService getNextChatMessageServiceA = _erector.Container.Resolve<IGetNextChatMessageService>();
                            //NOTE: Set up two readers 
                            messageBusReaderBankGetNextChangeMessageServiceA.AddAnotherReader(getNextChatMessageServiceA.ProcessMessage);
                            messageBusReaderBankGetNextChangeMessageServiceA.AddAnotherReader(getNextChatMessageServiceA.ProcessMessage);
                            getNextChatMessageServiceA.MessageBusReaderBank = messageBusReaderBankGetNextChangeMessageServiceA;

                            getNextChatMessageServiceA.MessageBusWiter = messageBusWriterGetNextChatMessageServiceA;
                            getNextChatMessageServiceA.MessageBusBank = _messageBusBankServices;
                            ITack tackGetNextChatMessageServiceA = _erector.Container.Resolve<ITack>();
                            tackGetNextChatMessageServiceA.SkyWatch = skyWatchA;
                            getNextChatMessageServiceA.Tack = tackGetNextChatMessageServiceA;

                            IRoute<string> routeGetNextChatMessageServiceA = _erector.Container.Resolve<IRoute<string>>();
                            routeGetNextChatMessageServiceA.Route = String.Format("{0}.{1}", routingServiceRouterA.RoutingServiceGUID, ChatServiceNames.GetNextChatMessageService);
                            routeGetNextChatMessageServiceA.RegisterRouteHandler = getNextChatMessageServiceA.HandleMessageFromRouter;
                            routingServiceRouterA.RegisterRoute(routeGetNextChatMessageServiceA);

                            _messageBusBankServices.RegisterMessageBus(getNextChatMessageServiceA.ServiceGUID, messageBusGetNextChatMessageServiceA);
                            _serviceList.Add(getNextChatMessageServiceA);


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
