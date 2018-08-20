using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Routing;
using SharedServices.Models.Constants;
using SharedServices.Services.IOC;
using SharedUtilities.Interfaces.Marshall;

namespace SharedServices.UnitTests.Routing
{
    [TestClass]
    public class MessageBusUnitTests
    {
        private ErectDIContainer _erector { get; set; }

        public MessageBusUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        [TestMethod]
        public void TestMessageBusGUID()
        {
            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();
            string messageBusGUID = messageBus.MessageBusGUID;
            Assert.IsFalse(String.IsNullOrEmpty(messageBusGUID));
        } 

        [TestMethod]
        public void TestValidateMessage()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IEnvelope envelope = _erector.Container.Resolve<IEnvelope>();
            IChatMessageEnvelope chatMessagesEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();

            //NOTE: Set up a valid envelope
            envelope.ClientProxyGUID = Guid.NewGuid().ToString();
            envelope.ServiceRoute = ChatServiceNames.ChatMessageService;
            envelope.RequestMethod = "GET";
            string message = marshaller.MarshallPayloadJSON(envelope);
            bool isValid = messageBus.ValidateMessage(message, envelope.GetMyJSONSchema());
            Assert.IsTrue(isValid);

            isValid = messageBus.ValidateMessage(message, chatMessagesEnvelope.GetMyJSONSchema());
            Assert.IsFalse(isValid);
        }
         
        [TestMethod]
        public void TestSendingReceivingMessageAndIsEmpty()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IEnvelope envelope = _erector.Container.Resolve<IEnvelope>();
            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();

            //NOTE: Set up a valid envelope
            envelope.RequestMethod = "GET";
            envelope.ClientProxyGUID = Guid.NewGuid().ToString();
            envelope.ServiceRoute = ChatServiceNames.ChatMessageService;
            string message = marshaller.MarshallPayloadJSON(envelope);

            Assert.IsTrue(messageBus.IsEmpty());
            try
            {
                messageBus.SendMessage(null);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBus.ExceptionMessage_MessageCannotBeNull);
            }
            try
            {
                messageBus.SendMessage(message);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBus.ExceptionMessage_JSONSchemaCannotBeNullOrEmpty);
            }
            messageBus.JsonSchema = (messagebusmessage) => envelope.GetMyJSONSchema();
            messageBus.SendMessage(message);
            
            Assert.IsFalse(messageBus.IsEmpty());
            string messageReceived = messageBus.ReceiveMessage();
            Assert.IsTrue(messageBus.IsEmpty());
            Assert.AreEqual(message, messageReceived);
        }
    }
}
