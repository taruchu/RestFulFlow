using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedServices.Interfaces.Envelope;
using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.Routing;
using SharedServices.Models.Constants;
using SharedServices.Services.IOC;

namespace SharedServices.UnitTests.Routing
{
    [TestClass]
    public class RoutingServiceUnitTest
    {
        private ErectDIContainer _erector { get; set; }
        private const string _TEST_MESSAGE = "testing 123.";

        public RoutingServiceUnitTest()
        {
            _erector = new ErectDIContainer();
        }

        private IMessageBusReaderBank<T> GetMockedMessageBusReaderBank<T>()
        {
            var mockedMessageBusReaderBank = new Mock<IMessageBusReaderBank<T>>();
            mockedMessageBusReaderBank
                .Setup(messageBusReaderBank => messageBusReaderBank.SpecifyTheMessageBus(It.IsAny<IMessageBus<T>>()))
                .Returns(() => "a GUID");
            mockedMessageBusReaderBank
                .Setup(messageBusReaderBank => messageBusReaderBank.AddAnotherReader(It.IsAny<Action<T>>()))
                .Returns(() => 1234);
            mockedMessageBusReaderBank
                .Setup(messageBusReaderBank => messageBusReaderBank.DecreaseReaderBank(It.IsAny<int>()))
                .Returns(() => 1234);           
            mockedMessageBusReaderBank
                .Setup(messageBusReaderBank => messageBusReaderBank.StopReading())
                .Returns(() => true);
            return mockedMessageBusReaderBank.Object;                
        }
        private IMessageBusWriter<T> GetMockedMessageBusWriter<T>()
        {
            var mockedIMessageBusWriter = new Mock<IMessageBusWriter<T>>();
            mockedIMessageBusWriter
                .Setup(messageBusWriter => messageBusWriter.SpecifyTheMessageBus(It.IsAny<IMessageBus<T>>()))
                .Returns(() => "a GUID");
            mockedIMessageBusWriter
                .Setup(messageBusWriter => messageBusWriter.Write(It.IsAny<T>()))
                .Returns(() => true);
            return mockedIMessageBusWriter.Object;
        }
        private IRoutingTable<T> GetMockedRoutingTable<T>()
        { 
            IMessageBusBank<T> messageBusBank = _erector.Container.Resolve<IMessageBusBank<T>>();
            var mockedRoutingTable = new Mock<IRoutingTable<T>>();
            mockedRoutingTable
                .Setup(routingTable => routingTable.RoutingTableGUID)
                .Returns("15242");
            mockedRoutingTable
                .SetupProperty(routingTable => routingTable.MessageBusBank, messageBusBank);
            mockedRoutingTable
                .Setup(routingTable => routingTable.RegisterRoute(It.IsAny<string>(), It.IsAny<Action<T>>()))
                .Returns(() => true);
            mockedRoutingTable
                .Setup(routingTable => routingTable.ResolveRoute(It.IsAny<string>()))
                .Returns(() => (message) => { });
            mockedRoutingTable
                .Setup(routingTable => routingTable.ReleaseRoute(It.IsAny<string>()))
                .Returns(() => true);
            return mockedRoutingTable.Object;
        }

        [TestMethod]
        public void TestRoutingServiceGUID()
        {
            IRoutingService<string> routingService = _erector.Container.Resolve<IRoutingService<string>>();
            routingService.RoutingTable = GetMockedRoutingTable<string>();
            string routingServiceGUID = routingService.RoutingServiceGUID;
            Assert.IsFalse(String.IsNullOrEmpty(routingServiceGUID));
        }

        [TestMethod]
        public void TestForwardMessageToResolvedRoute()
        {
            IRoutingService<string> routingService = _erector.Container.Resolve<IRoutingService<string>>();
            bool forwardSucceeded = false;
            Action<string> resolvedRoute = 
                (message) => 
                {
                    Assert.IsNotNull(message);//NOTE: Assert on route destination.
                };
            string jsonMessage = "123 Love";

            try
            {
                forwardSucceeded = routingService.ForwardMessageToResolvedRoute(null, jsonMessage);
            }
            catch(InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_ResolvedRouteCannotBeNull);
            }
            try
            {
                forwardSucceeded = routingService.ForwardMessageToResolvedRoute(resolvedRoute, String.Empty);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_MessageCannotBeNullOrEmpty);
            }
            forwardSucceeded = routingService.ForwardMessageToResolvedRoute(resolvedRoute, jsonMessage);
            Assert.IsTrue(forwardSucceeded);
        }

        [TestMethod]
        public void TestParseMessageForRoute()
        {
            IRoutingService<string> routingService = _erector.Container.Resolve<IRoutingService<string>>();
            IEnvelope envelope = _erector.Container.Resolve<IEnvelope>(); 
            envelope.InitializeThisEnvelopeFor_RoutingService();
            string destinationRoute = "123.789";
            envelope.Header_KeyValues[JSONSchemas.DestinationRoute] = destinationRoute;
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            string jsonMessage = marshaller.MarshallPayloadJSON(envelope);
            string parsedRoute;

            try
            {
              parsedRoute = routingService.ParseMessageForRoute(String.Empty);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_MessageCannotBeNullOrEmpty);
            }
            routingService.Marshaller = null;
            try
            {
                parsedRoute = routingService.ParseMessageForRoute(jsonMessage);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_MarshallerCannotBeNull);
            }
            routingService.Marshaller = _erector.Container.Resolve<IMarshaller>();
            parsedRoute = routingService.ParseMessageForRoute(jsonMessage);
            Assert.AreEqual(parsedRoute, destinationRoute);
        }

        [TestMethod]
        public void TestRegisterResolveReleaseRoute()
        {
            IRoutingService<string> routingService = _erector.Container.Resolve<IRoutingService<string>>();
            string destinationRoute = "123.789";
            IRoute<string> iRoute = _erector.Container.Resolve<IRoute<string>>();
            iRoute.Route = destinationRoute;
            iRoute.RegisterRouteHandler = (message) => { };            
            bool registerRoute = false;
            Action<string> resolveRoute = null;
            bool releaseRoute = false; 

            //RoutingTable Exceptions
            routingService.RoutingTable = null;
            try
            {
                registerRoute = routingService.RegisterRoute(iRoute);                
            }
            catch(InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_RoutingTableCannotBeNull);
            }
            try
            {
                resolveRoute = routingService.ResolveRoute(destinationRoute); 
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_RoutingTableCannotBeNull);
            }
            try
            {
                releaseRoute = routingService.ReleaseRoute(iRoute); 
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_RoutingTableCannotBeNull);
            }


            routingService.RoutingTable = GetMockedRoutingTable<string>(); 
            //Register
            try
            {
                registerRoute = routingService.RegisterRoute(null);
            }
            catch(InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_IRouteCannotBeNull);
            }            
            registerRoute = routingService.RegisterRoute(iRoute);
            Assert.IsTrue(registerRoute); 

            //Resolve
            try
            {
                resolveRoute = routingService.ResolveRoute(String.Empty);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_RouteCannotBeNullOrEmpty);
            }
            resolveRoute = routingService.ResolveRoute(destinationRoute);
            Assert.IsNotNull(resolveRoute);

            //Release
            try
            {
                releaseRoute = routingService.ReleaseRoute(null);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_IRouteCannotBeNull);
            }
            releaseRoute = routingService.ReleaseRoute(iRoute);
            Assert.IsTrue(releaseRoute); 
        }

        [TestMethod]
        public void TestInitializeReaders()
        {
            IRoutingService<string> routingService = _erector.Container.Resolve<IRoutingService<string>>();
            bool InitReadersSucceeded = false;

            //Initialize Readers
            routingService.MessageBusReaderBank = null;
            try
            {
                InitReadersSucceeded = routingService.InitializeReaders(1);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingService.ExceptionMessage_MessageBusReaderBankCannotBeNull);
            }
            routingService.MessageBusReaderBank = GetMockedMessageBusReaderBank<string>();
            InitReadersSucceeded = routingService.InitializeReaders(1);
            Assert.IsTrue(InitReadersSucceeded);
        }
    }
}
