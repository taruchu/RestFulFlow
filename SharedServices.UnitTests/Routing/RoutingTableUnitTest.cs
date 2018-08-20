using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedInterfaces.Interfaces.Routing;
using SharedServices.Services.IOC;

namespace SharedServices.UnitTests.Routing
{
    [TestClass]
    public class RoutingTableUnitTest
    {
        private ErectDIContainer _erector { get; set; }

        public RoutingTableUnitTest()
        {
            _erector = new ErectDIContainer();
        }



        [TestMethod]
        public void TestRoutingTableGUID()
        {
            IRoutingTable<string> routingTable = _erector.Container.Resolve<IRoutingTable<string>>();
            string routingTableGUID = routingTable.RoutingTableGUID;
            Assert.IsFalse(String.IsNullOrEmpty(routingTableGUID));
        }

        [TestMethod]
        public void TestRegisterResolveReleaseRoute()
        {
            IRoutingTable<string> routingTable = _erector.Container.Resolve<IRoutingTable<string>>();
            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();
            var mockedMessageBusBank = new Mock<IMessageBusBank<string>>();
            mockedMessageBusBank
                .Setup(messageBusBank => messageBusBank.RegisterMessageBus(It.IsAny<string>(), It.IsAny<IMessageBus<string>>()))
                .Returns(() => true);
            mockedMessageBusBank
                .Setup(messageBusBank => messageBusBank.ResolveMessageBus(It.IsAny<string>()))
                .Returns(() => messageBus);
            mockedMessageBusBank
                .Setup(messageBusBank => messageBusBank.ReleaseMessageBus(It.IsAny<string>()))
                .Returns(() => true);
            string route = String.Format("{0}.4B39F260-A40A-4673-A67B-6CECCE74DBB4", routingTable.RoutingTableGUID);
            Action<string> testAction = (message) => { };
            bool registerRoute = false;
            Action<string> resolveRoute = null;
            bool releaseRoute = false;

            
            try
            {
                routingTable.RegisterRoute("1234", testAction);
            }
            catch(InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingTable.ExceptionMessage_RouteFormatIsIncorrect);
            }
            try
            {
                routingTable.RegisterRoute(String.Empty, testAction);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingTable.ExceptionMessage_RouteCannotBeNullOrEmpty);
            }
            try
            {
                routingTable.RegisterRoute(route, null);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingTable.ExceptionMessage_RouteActionCannotBeNull);
            }
            registerRoute = routingTable.RegisterRoute(route, testAction);
            Assert.IsTrue(registerRoute);

            
            try
            {
                resolveRoute = routingTable.ResolveRoute("123");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingTable.ExceptionMessage_RouteFormatIsIncorrect);
            }
            try
            {
                resolveRoute = routingTable.ResolveRoute(String.Empty);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingTable.ExceptionMessage_RouteCannotBeNullOrEmpty);
            }
            try
            {
                resolveRoute = routingTable.ResolveRoute(route);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingTable.ExceptionMessage_MessageBusBankCannotBeNull);
            }
            routingTable.MessageBusBank = mockedMessageBusBank.Object;
            resolveRoute = routingTable.ResolveRoute(route);
            Assert.IsNotNull(resolveRoute);

            
            try
            {
                releaseRoute = routingTable.ReleaseRoute("123");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingTable.ExceptionMessage_RouteFormatIsIncorrect);
            }
            try
            {
                releaseRoute = routingTable.ReleaseRoute(String.Empty);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, routingTable.ExceptionMessage_RouteCannotBeNullOrEmpty);
            }           
            releaseRoute = routingTable.ReleaseRoute(route);
            Assert.IsTrue(releaseRoute);
        }
    }
}
