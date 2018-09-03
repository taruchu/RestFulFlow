using SharedInterfaces.Interfaces.IOC;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Models.EnvelopeModel;
using SharedInterfaces.Models.Envelope;
using DataPersistence.Interfaces;
using SharedUtilities.Interfaces.Marshall;
using SharedUtilities.Implementation.Marshall;
using DataPersistence.Interfaces.Files;
using DataPersistence.Services.Files;
using DataPersistence.Services.Configuration;
using DataPersistence.Interfaces.Configuration;

namespace DataPersistence.Services.IOC
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
                .Register<IEnvelope, EnvelopeModel>()
                .Register<IEnvelopeFactory, EnvelopeFactory>()
                .Register<IChatMessageEnvelope, ChatMessageEnvelope>()
                .Register<IChatMessageEnvelopeFactory, ChatMessageEnvelopeFactory>()
                .Register<IMarshaller, Marshaller>()
              
            
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
