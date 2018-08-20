using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedInterfaces.Interfaces.Routing;
using SharedServices.Services.IOC;

namespace SharedServices.UnitTests.Routing
{
    [TestClass]
    public class MessageBusReaderBankUnitTests
    {
        private ErectDIContainer _erector { get; set; }
        private object _thisLock { get; set; }

        public MessageBusReaderBankUnitTests()
        {
            _erector = new ErectDIContainer();
            _thisLock = new object();
        }

        IMessageBus<T> GetMockedMessageBus<T>()
        {
            var mockedMessageBus = new Mock<IMessageBus<T>>();
            mockedMessageBus
                .Setup(messageBus => messageBus.MessageBusGUID)
                .Returns(() => { lock (_thisLock) { return "502C7CFC-6FFE-46DF-8A49-313CFA1CACA0"; } });
            mockedMessageBus
                .Setup(messageBus => messageBus.ReceiveMessage())
                .Returns(() =>
                {
                    lock(_thisLock)
                    {
                        return (typeof(T) == typeof(string)) ? (T)Convert.ChangeType("testing 123", typeof(T)) : default(T);
                    }
                }); 
            mockedMessageBus
                .Setup(messageBus => messageBus.IsEmpty())
                .Returns(() => { lock(_thisLock) { return false; } });
            return mockedMessageBus.Object;
        }

        [TestMethod]
        public void TestSpecifyTheMessageBus()
        {
            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            IMessageBus<string> messageBus = GetMockedMessageBus<string>();
            string messageBusGUID = null;
            try
            {
                messageBusGUID = messageBusReaderBank.SpecifyTheMessageBus(null);
            }
            catch(InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusReaderBank.ExceptionMessage_MessageBusCannotBeNull);
            } 
            messageBusGUID = messageBusReaderBank.SpecifyTheMessageBus(messageBus);
            Assert.IsFalse(String.IsNullOrEmpty(messageBusGUID));
        }

        [TestMethod]
        public void TestAddAnotherReader()
        {
            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            IMessageBus<string> messageBus = GetMockedMessageBus<string>(); 
            int readerCount = 0; 
            try
            {
                readerCount = messageBusReaderBank.AddAnotherReader((message) => { });
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusReaderBank.ExceptionMessage_MessageBusCannotBeNull);
            }
            string messageBusGUID = messageBusReaderBank.SpecifyTheMessageBus(messageBus);
            Assert.IsFalse(String.IsNullOrEmpty(messageBusGUID));
            try
            {
                readerCount = messageBusReaderBank.AddAnotherReader(null);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBusReaderBank.ExceptionMessage_ActionPerformedOnEachMessageReadCannotBeNull);
            } 
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { });
            Assert.IsTrue(readerCount == 1);
            messageBusReaderBank.Dispose();
        }

        [TestMethod]
        public void TestDecreaseReaderBank()
        {
            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            IMessageBus<string> messageBus = GetMockedMessageBus<string>();
            string messageBusGUID = messageBusReaderBank.SpecifyTheMessageBus(messageBus);
            Assert.IsFalse(String.IsNullOrEmpty(messageBusGUID));
            int readerCount = 0;
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { });
            Assert.IsTrue(readerCount == 1);
            readerCount = messageBusReaderBank.DecreaseReaderBank(-2);
            Assert.IsTrue(readerCount == 1);
            readerCount = messageBusReaderBank.DecreaseReaderBank(23484);
            Assert.IsTrue(readerCount == 1);
            readerCount = messageBusReaderBank.DecreaseReaderBank(0);
            Assert.IsTrue(readerCount == 1);
            readerCount = messageBusReaderBank.DecreaseReaderBank(1);
            Assert.IsTrue(readerCount == 0);
        }

        [TestMethod]
        public void TestStopReading()
        {
            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            IMessageBus<string> messageBus = GetMockedMessageBus<string>();
            string messageBusGUID = messageBusReaderBank.SpecifyTheMessageBus(messageBus);
            Assert.IsFalse(String.IsNullOrEmpty(messageBusGUID));
            int readerCount = 0;
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(1); });
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(2); });
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(3); });
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(4); });
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(5); });
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(6); });
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(7); });
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(8); });
            readerCount = messageBusReaderBank.AddAnotherReader((message) => { Debug.WriteLine(9); });
            bool stopSuccessful = messageBusReaderBank.StopReading();
            Assert.IsTrue(stopSuccessful);
        } 
    }
}
