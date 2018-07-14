using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedServices.Interfaces.Routing;
using SharedServices.Services.IOC;

namespace SharedServices.UnitTests.Routing
{
    [TestClass]
    public class MessageBusBankUnitTests
    {
        private ErectDIContainer _erector { get; set; }
        public MessageBusBankUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        [TestMethod]
        public void TestMessageBusBankGUID()
        {
            IMessageBusBank<string> messageBusBank = _erector.Container.Resolve<IMessageBusBank<string>>();
            string messageBusBankGUID = messageBusBank.MessageBusBankGUID;
            Assert.IsFalse(String.IsNullOrEmpty(messageBusBankGUID));
        }

        [TestMethod]
        public void TestRegisterResolveRelaseMessageBus()
        {
            IMessageBusBank<string> messageBusBank = _erector.Container.Resolve<IMessageBusBank<string>>();
            bool addMessageBus = false;
            bool releaseMessageBus = false;
            IMessageBus<string> resolveMessageBus = null; 
            string busKeyCode = "123";

            IMessageBus<string> messageBus = null;
            try
            {
                addMessageBus = messageBusBank.RegisterMessageBus(busKeyCode, messageBus);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusBank.ExceptionMessage_MessgeBusCannotBeNull);
            }
            messageBus = _erector.Container.Resolve<IMessageBus<string>>();
            try
            {
                addMessageBus = messageBusBank.RegisterMessageBus(String.Empty, messageBus);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusBank.ExceptionMessage_BusKeyCodeCannotBeNullOrEmpty);
            } 
            addMessageBus = messageBusBank.RegisterMessageBus(busKeyCode, messageBus);
            Assert.IsTrue(addMessageBus);
            

            try
            {
                resolveMessageBus = messageBusBank.ResolveMessageBus(String.Empty);
            }
            catch(InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusBank.ExceptionMessage_BusKeyCodeCannotBeNullOrEmpty);
            }
            resolveMessageBus = messageBusBank.ResolveMessageBus(busKeyCode);
            Assert.IsNotNull(resolveMessageBus);
            Assert.AreEqual(messageBus.MessageBusGUID, resolveMessageBus.MessageBusGUID);


            try
            {
                releaseMessageBus = messageBusBank.ReleaseMessageBus(String.Empty);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusBank.ExceptionMessage_BusKeyCodeCannotBeNullOrEmpty);
            } 
            releaseMessageBus = messageBusBank.ReleaseMessageBus(busKeyCode);
            Assert.IsTrue(releaseMessageBus);
        }
    }
}
