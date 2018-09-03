using System;
using DataPersistence.Models.ChatMessage;
using DataPersistence.Services.Configuration;
using DataPersistence.Services.IOC;
using DataPersistence.Services.SQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Models.Envelope;

namespace DataPersistence.UnitTests
{
    [TestClass]
    public class SQLDataBaseBoardChatMessageUnitTest
    {
        private ErectDIContainer _erector { get; set; }

        public SQLDataBaseBoardChatMessageUnitTest()
        {
            _erector = new ErectDIContainer();
        }

        [TestMethod]
        public void TestMaptoEnvelope()
        {
            SQLDataBaseBoardChatMessage sQLDataBaseBoardChatMessage = new SQLDataBaseBoardChatMessage(new SQLDBConfigurationProvider(), new ChatMessageEnvelopeFactory());
            ChatMessage chatMessage = new ChatMessage();
            Channel channel = new Channel();
            chatMessage.ChannelID = 123;
            chatMessage.ChatMessageID = 147;
            chatMessage.Channel = channel;
            IChatMessageEnvelope chatMessageEnvelope = sQLDataBaseBoardChatMessage.MaptoEnvelope(chatMessage);
            Assert.IsNotNull(chatMessageEnvelope);
            Assert.AreEqual(chatMessageEnvelope.ChatMessageID, chatMessage.ChatMessageID);
            Assert.AreEqual(chatMessageEnvelope.ChatChannelID, chatMessage.ChannelID);
        }

        [TestMethod]
        public void TestMapToChatMessage()
        {
            SQLDataBaseBoardChatMessage sQLDataBaseBoardChatMessage = new SQLDataBaseBoardChatMessage(new SQLDBConfigurationProvider(), new ChatMessageEnvelopeFactory());
            IChatMessageEnvelope chatMessageEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            chatMessageEnvelope.ChatMessageID = 236;
            chatMessageEnvelope.ChatChannelName = "love";

            ChatMessage chatMessage = sQLDataBaseBoardChatMessage.MapToChatMessage(chatMessageEnvelope);
            Assert.IsNotNull(chatMessage);
            Assert.AreEqual(chatMessage.ChatMessageID, chatMessageEnvelope.ChatMessageID);
            Assert.AreEqual(chatMessage.ChannelID, chatMessageEnvelope.ChatChannelID);
        }

        [TestMethod]
        public void TestSQLDataBaseBoardCRUD()
        {
            SQLDataBaseBoardChatMessage sQLDataBaseBoardChatMessage = new SQLDataBaseBoardChatMessage(new SQLDBConfigurationProvider(), new ChatMessageEnvelopeFactory());
            IChatMessageEnvelope chatMessageEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            chatMessageEnvelope.ChatChannelName = "love";
            chatMessageEnvelope.ChatMessageBody = "Jesus Loves You.";
            

            //Create
            IChatMessageEnvelope newChatMessageEnvelope = (IChatMessageEnvelope)sQLDataBaseBoardChatMessage.POST(chatMessageEnvelope);
            Assert.IsNotNull(newChatMessageEnvelope);
            Assert.IsNotNull(newChatMessageEnvelope.CreatedDateTime);
            newChatMessageEnvelope.Query = (queryRepo, envelope) =>
            {
                return queryRepo.GetChatMessageByID(envelope);
            };

            //Read
            IChatMessageEnvelope readChatMessageEnvelope = (IChatMessageEnvelope)sQLDataBaseBoardChatMessage.GET(newChatMessageEnvelope);
            Assert.IsNotNull(readChatMessageEnvelope);
            Assert.AreEqual(readChatMessageEnvelope.ChatMessageID, newChatMessageEnvelope.ChatMessageID);

            //Update            
            IChatMessageEnvelope updateChatMessageEnvelope = (IChatMessageEnvelope)sQLDataBaseBoardChatMessage.PUT(readChatMessageEnvelope);
            Assert.IsNotNull(updateChatMessageEnvelope);
            Assert.IsTrue(DateTime.Compare(readChatMessageEnvelope.ModifiedDateTime, updateChatMessageEnvelope.ModifiedDateTime) < 0);

            //Delete
            IChatMessageEnvelope deletedChatMessageEnvelope = (IChatMessageEnvelope)sQLDataBaseBoardChatMessage.DELETE(updateChatMessageEnvelope);
            Assert.IsNotNull(updateChatMessageEnvelope);
            Assert.IsTrue(DateTime.Compare(updateChatMessageEnvelope.ModifiedDateTime, deletedChatMessageEnvelope.ModifiedDateTime) < 0);
        }

        [TestMethod]
        public void TestGetNextChatMessage()
        {
            SQLDataBaseBoardChatMessage sQLDataBaseBoardChatMessage = new SQLDataBaseBoardChatMessage(new SQLDBConfigurationProvider(), new ChatMessageEnvelopeFactory());
            IChatMessageEnvelope sameChannelChatMessageEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            sameChannelChatMessageEnvelope.ChatChannelName = "love";
            sameChannelChatMessageEnvelope.ChatMessageBody = "Jesus Loves You.";
            sameChannelChatMessageEnvelope.ChatChannelID = 1;
            IChatMessageEnvelope newChatMessageEnvelope = (IChatMessageEnvelope)sQLDataBaseBoardChatMessage.POST(sameChannelChatMessageEnvelope);

            IChatMessageEnvelope chatMessageEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            chatMessageEnvelope.ChatMessageID = 1;
            chatMessageEnvelope.Query = (queryRepo, envelope) =>
            {
                return queryRepo.GetChatMessageByID(envelope);
            };

            IChatMessageEnvelope readChatMessageEnvelope = (IChatMessageEnvelope)sQLDataBaseBoardChatMessage.GET(chatMessageEnvelope);
            IChatMessageEnvelope nextChatMessageEnvelope = sQLDataBaseBoardChatMessage.GetNextChatMessage(readChatMessageEnvelope);
            Assert.IsNotNull(nextChatMessageEnvelope);
            Assert.IsTrue(nextChatMessageEnvelope.ChatMessageID > readChatMessageEnvelope.ChatMessageID);
        }
    }
}
