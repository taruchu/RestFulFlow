using System;
using ChatMessageInterfaces.Interfaces.ChatMessage;
using DataPersistence.Interfaces;
using SharedServices.Services.IOC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Routing;
using SharedUtilities.Interfaces.Marshall;

namespace SharedServices.UnitTests.ChatMessage
{
    [TestClass]
    public class GetNextChatMessageServiceUnitTests
    {
        private ErectDIContainer _erector { get; set; }

        public GetNextChatMessageServiceUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        private ITack GetMockedITack()
        {
            var mockedITack = new Mock<ITack>();
            mockedITack
                .Setup(tack => tack.GET(It.IsAny<IEnvelope>()))
                .Returns<IEnvelope>((arg) =>
                {
                    //NOTE: Return the next chat message on this channel.
                    IChatMessageEnvelope newEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
                    newEnvelope.ChatMessageID = ((IChatMessageEnvelope)arg).ChatMessageID + 1;
                    newEnvelope.ChatChannelID = ((IChatMessageEnvelope)arg).ChatChannelID;
                    newEnvelope.CreatedDateTime = ((IChatMessageEnvelope)arg).CreatedDateTime.AddMinutes(5);
                    newEnvelope.ModifiedDateTime = DateTime.MinValue;
                    return newEnvelope;
                });
             
            return mockedITack.Object;
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
        public void TestGetNextChatMessageServiceSendResponse()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();

            IGetNextChatMessageService getNextChatMessageService = _erector.Container.Resolve<IGetNextChatMessageService>();

            string clientGuid = Guid.NewGuid().ToString();
            IMessageBus<string> messageBus_Client = _erector.Container.Resolve<IMessageBus<string>>();
            IMessageBus<string> messageBus_Service = _erector.Container.Resolve<IMessageBus<string>>();
            messageBus_Client.JsonSchema =
               (message) =>
               {
                   //NOTE: Require this schema for the client only since I'm not sending anything to the service.
                   return _erector.Container.Resolve<IChatMessageEnvelope>().GetMyJSONSchema();
               };

            IMessageBusBank<string> messageBusBank = _erector.Container.Resolve<IMessageBusBank<string>>();
            messageBusBank.RegisterMessageBus(getNextChatMessageService.ServiceGUID, messageBus_Service);
            messageBusBank.RegisterMessageBus(clientGuid, messageBus_Client);


            IChatMessageEnvelope requestEnvelope = GetValidChatMessageEnvelope();
            string requestPayload = marshaller.MarshallPayloadJSON(requestEnvelope);
            bool success = false;

            //MessageBusBank cannot be null
            try
            {
                success = getNextChatMessageService.SendResponse(clientGuid, requestPayload);
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(message, getNextChatMessageService.ExceptionMessage_MessageBusBankCannotBeNull);
            }
            success = false;

            //Client should recieve message in their message bus.
            getNextChatMessageService.MessageBusBank = messageBusBank;
            success = getNextChatMessageService.SendResponse(clientGuid, requestPayload);
            Assert.IsTrue(success);
            string clientMessage = messageBus_Client.ReceiveMessage();
            Assert.IsFalse(String.IsNullOrEmpty(clientMessage));
            Assert.AreEqual(requestPayload, clientMessage);
        }

        [TestMethod]
        public void TestGetNextChatMessageServiceGET()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();

            IGetNextChatMessageService getNextChatMessageService = _erector.Container.Resolve<IGetNextChatMessageService>();

            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();
            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            messageBusReaderBank.SpecifyTheMessageBus(messageBus);
            IMessageBusWriter<string> messageBusWriter = _erector.Container.Resolve<IMessageBusWriter<string>>();
            messageBusWriter.SpecifyTheMessageBus(messageBus);

            IMessageBusBank<string> messageBusBank = _erector.Container.Resolve<IMessageBusBank<string>>();
            messageBusBank.RegisterMessageBus(getNextChatMessageService.ServiceGUID, messageBus);

            getNextChatMessageService.MessageBusBank = messageBusBank;
            getNextChatMessageService.MessageBusReaderBank = messageBusReaderBank;
            getNextChatMessageService.MessageBusReaderBank.AddAnotherReader(getNextChatMessageService.ProcessMessage);
            getNextChatMessageService.MessageBusWiter = messageBusWriter;

            IChatMessageEnvelope requestEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            requestEnvelope.ChatMessageID = 123;

            //Test ITack cannot be null
            string tackCannotBeNull;
            try
            {
                tackCannotBeNull = getNextChatMessageService.Get(requestEnvelope);
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(message, getNextChatMessageService.ExceptionMessage_ITackCannotBeNull);
            }

            getNextChatMessageService.Tack = GetMockedITack();
            requestEnvelope = GetValidChatMessageEnvelope();
            requestEnvelope.CreatedDateTime = DateTime.Now;
            IChatMessageEnvelope responseEnvelope;

            //Read next message
            string read = getNextChatMessageService.Get(requestEnvelope);
            Assert.IsFalse(String.IsNullOrEmpty(read));
            responseEnvelope = marshaller.UnMarshall<IChatMessageEnvelope>(read);
            Assert.IsNotNull(responseEnvelope);
            Assert.IsTrue(responseEnvelope.ChatMessageID > requestEnvelope.ChatMessageID);
            Assert.AreEqual(responseEnvelope.ChatChannelID, requestEnvelope.ChatChannelID);
            Assert.IsTrue(DateTime.Compare(responseEnvelope.CreatedDateTime, requestEnvelope.CreatedDateTime) > 0);
        }
    }
}
