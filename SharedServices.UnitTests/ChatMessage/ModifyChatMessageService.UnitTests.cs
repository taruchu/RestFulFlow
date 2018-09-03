using ChatMessageInterfaces.Interfaces.ChatMessage;
using DataPersistence.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Routing;
using SharedServices.Services.IOC;
using SharedUtilities.Interfaces.Marshall;
using System;

namespace SharedServices.UnitTests.ChatMessage
{
    [TestClass]
    public class ModifyChatMessageServiceUnitTests
    {
        private ErectDIContainer _erector { get; set; }
        public ModifyChatMessageServiceUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        private ITack GetMockedITack()
        {
            var mockedITack = new Mock<ITack>();
            mockedITack
                .Setup(tack => tack.POST(It.IsAny<IEnvelope>()))
                .Returns<IEnvelope>((arg) =>
                    {
                        IChatMessageEnvelope newEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
                        newEnvelope.ChatMessageID = ((IChatMessageEnvelope)arg).ChatMessageID;
                        newEnvelope.CreatedDateTime = DateTime.Now;
                        newEnvelope.ModifiedDateTime = ((IChatMessageEnvelope)arg).ModifiedDateTime; 
                        return newEnvelope;
                    }); 
            mockedITack
               .Setup(tack => tack.PUT(It.IsAny<IEnvelope>()))
               .Returns<IEnvelope>((arg) =>
               {
                   IChatMessageEnvelope newEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
                   newEnvelope.ChatMessageID = ((IChatMessageEnvelope)arg).ChatMessageID;
                   newEnvelope.CreatedDateTime = ((IChatMessageEnvelope)arg).CreatedDateTime;
                   newEnvelope.ModifiedDateTime = DateTime.Now;
                   return newEnvelope;
               });
            mockedITack
               .Setup(tack => tack.DELETE(It.IsAny<IEnvelope>()))
               .Returns<IEnvelope>((arg) =>
               {
                   IChatMessageEnvelope newEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
                   newEnvelope.ChatMessageID = ((IChatMessageEnvelope)arg).ChatMessageID;
                   newEnvelope.CreatedDateTime = ((IChatMessageEnvelope)arg).CreatedDateTime;
                   newEnvelope.ModifiedDateTime = DateTime.Now;
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
        public void TestModifyChatMessageServiceCreateUpdateDelete()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();

            IModifyChatMessageService modifyChatMessageService = _erector.Container.Resolve<IModifyChatMessageService>();

            IMessageBus<string> messageBus = _erector.Container.Resolve<IMessageBus<string>>();
            IMessageBusReaderBank<string> messageBusReaderBank = _erector.Container.Resolve<IMessageBusReaderBank<string>>();
            messageBusReaderBank.SpecifyTheMessageBus(messageBus);
            IMessageBusWriter<string> messageBusWriter = _erector.Container.Resolve<IMessageBusWriter<string>>();
            messageBusWriter.SpecifyTheMessageBus(messageBus);

            IMessageBusBank<string> messageBusBank = _erector.Container.Resolve<IMessageBusBank<string>>();
            messageBusBank.RegisterMessageBus(modifyChatMessageService.ServiceGUID, messageBus);

            modifyChatMessageService.MessageBusBank = messageBusBank;
            modifyChatMessageService.MessageBusReaderBank = messageBusReaderBank;
            modifyChatMessageService.MessageBusReaderBank.AddAnotherReader(modifyChatMessageService.ProcessMessage);
            modifyChatMessageService.MessageBusWiter = messageBusWriter;
            
            IChatMessageEnvelope requestEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            requestEnvelope.ChatMessageID = 123;

            //Test ITack cannot be null
            string tackCannotBeNull;
            try
            {
                tackCannotBeNull = modifyChatMessageService.Post(requestEnvelope);
                Assert.Fail();
            }
            catch(InvalidOperationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(message, modifyChatMessageService.ExceptionMessage_ITackCannotBeNull);
            }
            try
            {
                tackCannotBeNull = modifyChatMessageService.Put(requestEnvelope);
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(message, modifyChatMessageService.ExceptionMessage_ITackCannotBeNull);
            }
            try
            {
                tackCannotBeNull = modifyChatMessageService.Delete(requestEnvelope);
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(message, modifyChatMessageService.ExceptionMessage_ITackCannotBeNull);
            }

            modifyChatMessageService.Tack = GetMockedITack();
            requestEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            IChatMessageEnvelope responseEnvelope;

            //Create
            string created = modifyChatMessageService.Post(requestEnvelope);
            Assert.IsFalse(String.IsNullOrEmpty(created));
            responseEnvelope = marshaller.UnMarshall<IChatMessageEnvelope>(created);
            Assert.AreEqual(responseEnvelope.ChatMessageID, requestEnvelope.ChatMessageID);
            Assert.IsTrue(DateTime.Compare(responseEnvelope.CreatedDateTime, DateTime.MinValue) > 0);
            requestEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            requestEnvelope.ChatMessageID = 123;


            //Update
            string updated = modifyChatMessageService.Put(requestEnvelope);
            Assert.IsFalse(String.IsNullOrEmpty(updated));
            responseEnvelope = marshaller.UnMarshall<IChatMessageEnvelope>(updated);
            Assert.AreEqual(responseEnvelope.ChatMessageID, requestEnvelope.ChatMessageID);
            Assert.IsTrue(DateTime.Compare(responseEnvelope.ModifiedDateTime, requestEnvelope.ModifiedDateTime) > 0);
            requestEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            requestEnvelope.ChatMessageID = 123;


            //Delete
            string deleted = modifyChatMessageService.Delete(requestEnvelope);
            Assert.IsFalse(String.IsNullOrEmpty(deleted));
            responseEnvelope = marshaller.UnMarshall<IChatMessageEnvelope>(deleted);
            Assert.AreEqual(responseEnvelope.ChatMessageID, requestEnvelope.ChatMessageID);
            Assert.IsTrue(DateTime.Compare(responseEnvelope.ModifiedDateTime, requestEnvelope.ModifiedDateTime) > 0);
        }

        [TestMethod]
        public void TestModifyChatMessageServiceSendResponse()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();

            IModifyChatMessageService modifyChatMessageService = _erector.Container.Resolve<IModifyChatMessageService>();
             
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
            messageBusBank.RegisterMessageBus(modifyChatMessageService.ServiceGUID, messageBus_Service);
            messageBusBank.RegisterMessageBus(clientGuid, messageBus_Client);


            IChatMessageEnvelope requestEnvelope = GetValidChatMessageEnvelope(); 
            string requestPayload = marshaller.MarshallPayloadJSON(requestEnvelope);
            bool success = false;

            //MessageBusBank cannot be null
            try
            {
                success = modifyChatMessageService.SendResponse(clientGuid, requestPayload);
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(message, modifyChatMessageService.ExceptionMessage_MessageBusBankCannotBeNull);
            }
            success = false;

            //Client should recieve message in their message bus.
            modifyChatMessageService.MessageBusBank = messageBusBank;
            success = modifyChatMessageService.SendResponse(clientGuid, requestPayload);
            Assert.IsTrue(success);
            string clientMessage = messageBus_Client.ReceiveMessage();
            Assert.IsFalse(String.IsNullOrEmpty(clientMessage));
            Assert.AreEqual(requestPayload, clientMessage);
        } 
    }
}
