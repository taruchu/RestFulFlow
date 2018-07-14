using SharedServices.Interfaces.ChatMessage;
using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.TCP;
using SharedServices.Interfaces.IOC;
using SharedServices.Services.ChatMessage;
using SharedServices.Services.Marshall;
using SharedServices.Services.TCP;
using SharedServices.Interfaces.Routing;
using SharedServices.Models.Routing;
using SharedServices.Services.Routing;
using SharedServices.Interfaces.Envelope;
using SharedServices.Models.EnvelopeModel;
using SharedServices.Interfaces.Transactions;
using SharedServices.Models.Transactions;
using SharedServices.Services.Transaction;

namespace SharedServices.Services.IOC
{
    class EstablishIOC : IIOCEstablishment
    {
        public EstablishIOC()
        { }
        public IIOCContainer EstablishContainer(IIOCFactory fromFactory)
        {
            return fromFactory.InstantiateContainer();
        }

        public void StandUp(IIOCContainer container) => container 
                .Register<IChatMessageService, ChatMessageService>()  
                .Register<IImplementationTypeResolver, ImplementationTypeResolver>() 
                .Register<ITCPAvailablePortsService, TCPAvailablePortsService>() 
                .Register<IEnvelope, EnvelopeModel>()
                .Register<IEnvelopeFactory, EnvelopeFactory>()
                .Register<IMarshaller, Marshaller>()
                .Register<ITransactionResult, TransactionResult>()
                .Register<ITransactionResultFactory, TransactionResultFactory>()


                .Register<IRoute<string>, Route<string>>()
                .Register<IRouteFactory<string>, RouteFactory<string>>()
                .Register<IMessageBus<string>, MessageBus<string>>()
                .Register<IMessageBusBank<string>, MessageBusBank<string>>()
                .Register<IRoutingTable<string>, RoutingTable<string>>()
                .Register<IRoutingService<string>, RoutingService<string>>()
                .Register<IMessageBusReaderBank<string>, MessageBusReaderBank<string>>()
                .Register<IMessageBusWriter<string>, MessageBusWriter<string>>()


                ;
    }
}
