using SharedInterfaces.Interfaces.ChatMessage;
using SharedInterfaces.Interfaces.TCP;
using SharedInterfaces.Interfaces.IOC;
using SharedServices.Services.ChatMessage;
using SharedServices.Services.TCP;
using SharedInterfaces.Interfaces.Routing;
using SharedServices.Models.Routing;
using SharedServices.Services.Routing;
using SharedInterfaces.Interfaces.Envelope;
using SharedServices.Models.EnvelopeModel;
using SharedInterfaces.Interfaces.Transactions;
using SharedServices.Models.Transactions;
using SharedServices.Services.Transaction;
using SharedServices.Models.Envelope;
using DataPersistence.Services;
using DataPersistence.Interfaces;
using SharedUtilities.Interfaces.Marshall;
using SharedUtilities.Implementation.Marshall;
using DataPersistence.Interfaces.Files;
using DataPersistence.Services.Files;
using DataPersistence.Services.Configuration;
using DataPersistence.Interfaces.Configuration;
using DataPersistence.Interfaces.SQL;
using DataPersistence.Services.SQL;

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
                .Register<IChatMessageEnvelope, ChatMessageEnvelope>()
                .Register<IChatMessageEnvelopeFactory, ChatMessageEnvelopeFactory>()
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

                .Register<ITack, Tack>()
                .Register<IBoards, Boards>()
                .Register<ISkyWatch, SkyWatch>()
                .Register<IFileStorage, FileStorage>()
                .Register<IDataInMemoryCache<IEnvelope>, DataInMemoryCache<IEnvelope>>()
                .Register<ISQLDBConfigurationProvider, SQLDBConfigurationProvider>()
                .Register<ISQLDBConfiguration, SQLDBConfiguration>()  


                ;
    }
}
