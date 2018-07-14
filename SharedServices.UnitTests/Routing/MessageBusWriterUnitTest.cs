using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedServices.Interfaces.Routing;
using SharedServices.Services.IOC;

namespace SharedServices.UnitTests.Routing
{
    [TestClass]
    public class MessageBusWriterUnitTest
    {
        private ErectDIContainer _erector { get; set; }

        public MessageBusWriterUnitTest()
        {
            _erector = new ErectDIContainer();
        }

        IMessageBus<T> GetMockedMessageBus<T>()
        {
            var mockedMessageBus = new Mock<IMessageBus<T>>();
            mockedMessageBus
                .Setup(messageBus => messageBus.MessageBusGUID)
                .Returns("EE98379D-AB78-498D-A4FA-D29B181C71BC");
            mockedMessageBus
                .Setup(messageBus => messageBus.SendMessage(It.IsAny<T>()))
                .Returns(true);
            return mockedMessageBus.Object;                
        }

        [TestMethod]
        public void TestSpecifyTheMessageBus()
        {
            IMessageBusWriter<string> messageBusWriter = _erector.Container.Resolve<IMessageBusWriter<string>>();
            string messageBusGUID = null;
            try
            {
                messageBusGUID = messageBusWriter.SpecifyTheMessageBus(null); 
            }
            catch(InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusWriter.ExceptionMessage_MessageBusCannotBeNull);
            }
            IMessageBus<string> messageBus = GetMockedMessageBus<string>();
            messageBusGUID = messageBusWriter.SpecifyTheMessageBus(messageBus);
            Assert.IsFalse(String.IsNullOrEmpty(messageBusGUID));
            messageBusWriter.Dispose();
        }

        [TestMethod]
        public void TestWrite()
        {
            IMessageBusWriter<string> messageBusWriter = _erector.Container.Resolve<IMessageBusWriter<string>>();
            string messageBusGUID = null;
            IMessageBus<string> messageBus = GetMockedMessageBus<string>(); 
            bool writeSucceeded = false;
            string message = "Jesus Loves You.";

            try
            {
                writeSucceeded = messageBusWriter.Write(message);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusWriter.ExceptionMessage_MessageBusCannotBeNull);
            }
            messageBusGUID = messageBusWriter.SpecifyTheMessageBus(messageBus);
            Assert.IsFalse(String.IsNullOrEmpty(messageBusGUID));
            try
            {
                writeSucceeded = messageBusWriter.Write(String.Empty);
            }
            catch(InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusWriter.ExceptionMessage_MessageCannotBeNullOrEmpty);
            } 
            writeSucceeded = messageBusWriter.Write(message);
            Assert.IsTrue(writeSucceeded);
            messageBusWriter.Dispose();
        }
    }
}
