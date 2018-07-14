using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedServices.Interfaces.Envelope;
using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.Routing;
using SharedServices.Models.Constants;
using SharedServices.Services.IOC;

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
            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>(); 

            envelope.InitializeThisEnvelopeFor_RoutingService();
            string message = marshaller.MarshallPayloadJSON(envelope);
            bool isValid = messageBus.ValidateMessage(message, JSONSchemas.RoutingServiceSchema);
            Assert.IsTrue(isValid);

            isValid = messageBus.ValidateMessage(message, JSONSchemas.ChatMessageServiceSchema);
            Assert.IsFalse(isValid);
        }
         
        [TestMethod]
        public void TestSendingReceivingMessageAndIsEmpty()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IEnvelope envelope = _erector.Container.Resolve<IEnvelope>();
            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();

            envelope.InitializeThisEnvelopeFor_RoutingService();
            string message = marshaller.MarshallPayloadJSON(envelope);

            Assert.IsTrue(messageBus.IsEmpty());

            try
            {
                messageBus.SendMessage(message);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, messageBus.ExceptionMessage_JSONSchemaCannotBeNullOrEmpty);
            }
            messageBus.JsonSchema = JSONSchemas.RoutingServiceSchema;
            messageBus.SendMessage(message);
            
            Assert.IsFalse(messageBus.IsEmpty());
            string messageReceived = messageBus.ReceiveMessage();
            Assert.IsTrue(messageBus.IsEmpty());
            Assert.AreEqual(message, messageReceived);
        }
    }
}
