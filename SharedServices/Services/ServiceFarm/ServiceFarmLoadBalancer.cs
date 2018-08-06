using log4net;
using log4net.Config;
using SharedServices.Interfaces.ChatMessage;
using SharedServices.Interfaces.Envelope;
using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.Routing;
using SharedServices.Interfaces.ServiceFarm;
using SharedServices.Models.Constants;
using SharedServices.Services.IOC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
        private ConcurrentDictionary<string, string> _clientProxyRouters { get; set; }

        public ServiceFarmLoadBalancer()
        {
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(ConfigurationConstants.FileName_log4NetConfiguration));
            _thisLock = new object();
            _isDisposed = false;
            _serviceList = new List<IDisposable>();            
            _erector = new ErectDIContainer();
            _marshaller = _erector.Container.Resolve<IMarshaller>();
            _clientProxyRouters = new ConcurrentDictionary<string, string>();
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
                        messageBusRouterA.JsonSchema = JSONSchemas.RoutingServiceSchema;

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
                        messageBusChatMessageServiceA.JsonSchema = JSONSchemas.ChatMessageServiceSchema;

                        IMessageBusReaderBank<string> messageBusReaderBankChangeMessageServiceA = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
                        messageBusReaderBankChangeMessageServiceA.SpecifyTheMessageBus(messageBusChatMessageServiceA);

                        IMessageBusWriter<string> messageBusWriterChatMessageServiceA = _erector.Container.Resolve<IMessageBusWriter<string>>();
                        messageBusWriterChatMessageServiceA.SpecifyTheMessageBus(messageBusChatMessageServiceA);

                        IChatMessageService chatMessageServiceA = _erector.Container.Resolve<IChatMessageService>();
                        chatMessageServiceA.MessageBusReaderBank = messageBusReaderBankChangeMessageServiceA;
                        chatMessageServiceA.MessageBusWiter = messageBusWriterChatMessageServiceA;
                        chatMessageServiceA.Marshaller = _erector.Container.Resolve<IMarshaller>();

                        IRoute<string> routeChatMessageServiceA = _erector.Container.Resolve<IRoute<string>>();
                        routeChatMessageServiceA.Route = ChatServiceNames.ChatMessageService;
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
            if(_isDisposed == false)
            {
                foreach (IDisposable service in _serviceList)
                    service.Dispose();

                _messageBusBankServices.Dispose();
                _messageBusBankRouters.Dispose();

                _isDisposed = true;
            }
        }

        public bool SendServiceRequest(string clientProxyOrigin,  string requestEnvelope)
        {
            lock(_thisLock)
            {
                try
                {
                     
                    string routerDetermined = String.Empty;
                    string destinationRouteDetermined = String.Empty;                      

                    IEnvelope envelope = _marshaller.UnMarshall(requestEnvelope);
                    routerDetermined = envelope.Header_KeyValues[JSONSchemas.DestinationRoute].Split('.')[0];
                    if(String.IsNullOrEmpty(routerDetermined))
                    {
                        int routerIndex = LoadBalanceAlgorithm(_messageBusBankRouters.GetBusKeyCodes().Count);
                        routerDetermined = _messageBusBankRouters.GetBusKeyCodes()[routerIndex];
                    }
                        
                    string serviceNameRequested = envelope.Header_KeyValues[JSONSchemas.ServiceNameRequested];
                    destinationRouteDetermined = String.Format("{0}.{1}", routerDetermined, serviceNameRequested);
                    envelope.Header_KeyValues[JSONSchemas.DestinationRoute] = destinationRouteDetermined; 
                    envelope.Header_KeyValues[JSONSchemas.ClientProxyOrigin] = clientProxyOrigin;  

                    string saltedRequest = _marshaller.MarshallPayloadJSON(envelope);
                    _messageBusBankRouters.ResolveMessageBus(routerDetermined).SendMessage(saltedRequest);
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

        public bool SendRegistrationToRouterRequest(string clientProxyOrigin, Action<string> responseCallback)
        {
            lock (_thisLock)
            {
                int routerIndex = LoadBalanceAlgorithm(_messageBusBankRouters.GetBusKeyCodes().Count);
                string routerDetermined = _messageBusBankRouters.GetBusKeyCodes()[routerIndex];
                _clientProxyRouters.TryAdd(clientProxyOrigin, routerDetermined);
                string destinationRouteDetermined = String.Format("{0}.{1}", routerDetermined, clientProxyOrigin);

                IEnvelope register = _erector.Container.Resolve<IEnvelope>();
                register.InitializeThisEnvelopeFor_RoutingService();

                IRoute<string> route = _erector.Container.Resolve<IRoute<string>>();
                route.Route = destinationRouteDetermined;
                route.RegisterRouteHandler = responseCallback; 
                 
                register.Header_KeyValues[JSONSchemas.ClientProxyOrigin] = clientProxyOrigin;
                register.Header_KeyValues[JSONSchemas.DestinationRoute] = destinationRouteDetermined;
                register.Payload_KeyValues[JSONSchemas.Route] = _marshaller.MarshallPayloadJSON(route);
                register.Payload_KeyValues[JSONSchemas.RoutingServiceCommand] = JSONSchemas.RoutingServiceCommandRegister;

                string registrationEnvelope = _marshaller.MarshallPayloadJSON(register);
                _messageBusBankRouters.ResolveMessageBus(routerDetermined).SendMessage(registrationEnvelope);

                return true; 
            } 
        }

        public bool SendReleaseRegistrationToRouterRequest(string clientProxyOrigin)
        {
            lock (_thisLock)
            {
                string routerDetermined = String.Empty;
                if (_clientProxyRouters.TryGetValue(clientProxyOrigin, out routerDetermined) == false)
                    return false;

                string destinationRouteDetermined = String.Format("{0}.{1}", routerDetermined, clientProxyOrigin);
                IEnvelope register = _erector.Container.Resolve<IEnvelope>();
                register.InitializeThisEnvelopeFor_RoutingService();  
                register.Header_KeyValues[JSONSchemas.ClientProxyOrigin] = clientProxyOrigin;
                register.Header_KeyValues[JSONSchemas.DestinationRoute] = destinationRouteDetermined; 
                register.Payload_KeyValues[JSONSchemas.RoutingServiceCommand] = JSONSchemas.RoutingServiceCommandRelease;

                string registrationEnvelope = _marshaller.MarshallPayloadJSON(register);
                _messageBusBankRouters.ResolveMessageBus(routerDetermined).SendMessage(registrationEnvelope);

                return true;
            }
        }
    }
}
