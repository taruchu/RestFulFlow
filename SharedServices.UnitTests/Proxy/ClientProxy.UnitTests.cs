using System;
using SharedServices.Services.IOC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Proxy;
using SharedInterfaces.Interfaces.Routing;
using SharedUtilities.Interfaces.Marshall;

namespace SharedServices.UnitTests.Proxy
{
    [TestClass]
    public class ClientProxyUnitTests
    {
        private ErectDIContainer _erector { get; set; }

        public ClientProxyUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        private IChatMessageEnvelope GetValidChatMessageEnvelope()
        {
            IChatMessageEnvelope chatMessageEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            chatMessageEnvelope.ServiceRoute = "ServiceRoute";
            chatMessageEnvelope.ClientProxyGUID = Guid.NewGuid().ToString();
            chatMessageEnvelope.RequestMethod = "GET";
            chatMessageEnvelope.ChatMessageID = 1;
            chatMessageEnvelope.ChatChannelID = 1;
            chatMessageEnvelope.ChatChannelName = "AwesomeSoft";
            chatMessageEnvelope.SenderUserName = "Jesus";
            chatMessageEnvelope.ChatMessageBody = "I love you Dionn.";
            chatMessageEnvelope.CreatedDateTime = DateTime.MinValue;
            chatMessageEnvelope.ModifiedDateTime = DateTime.MinValue;
            return chatMessageEnvelope;
        }

        [TestMethod]
        public void TestClientProxyPollMessageBus()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();

            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();

            string clientGuid = Guid.NewGuid().ToString();
            IMessageBus<string> messageBus_Client = _erector.Container.Resolve<IMessageBus<string>>();
            messageBus_Client.SkipValidation = true;                
             
            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            messageBusReaderBank.SpecifyTheMessageBus(messageBus_Client);

            clientProxy.MessageBusReaderBank = messageBusReaderBank;

            IChatMessageEnvelope requestEnvelope = GetValidChatMessageEnvelope();
            string requestPayload = marshaller.MarshallPayloadJSON(requestEnvelope);

            //Send a message
            messageBus_Client.SendMessage(requestPayload);

            //Poll the client proxy for that message
            string message = clientProxy.PollMessageBus(new System.Threading.CancellationTokenSource());
            Assert.IsFalse(String.IsNullOrEmpty(message));
            Assert.AreEqual(message, requestPayload);
        }
    }
}
