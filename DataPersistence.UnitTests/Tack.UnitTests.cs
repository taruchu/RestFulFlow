using System;
using DataPersistence.Interfaces;
using DataPersistence.Interfaces.SQL;
using DataPersistence.Services;
using DataPersistence.Services.IOC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Models.Envelope;

namespace DataPersistence.UnitTests
{
    [TestClass]
    public class TackUnitTests
    {
        private ErectDIContainer _erector { get; set; }
        public TackUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        private ISQLDataBaseBoardChatMessage GetMockedISQLDataBaseBoardChatMessage()
        {
            var mockedIBoard = new Mock<ISQLDataBaseBoardChatMessage>();
            mockedIBoard
                .Setup(board => board.GET(It.IsAny<IEnvelope>()))
                .Returns<IEnvelope>((arg) => arg);
            mockedIBoard
               .Setup(board => board.POST(It.IsAny<IEnvelope>()))
               .Returns<IEnvelope>((arg) => arg);
            mockedIBoard
               .Setup(board => board.PUT(It.IsAny<IEnvelope>()))
               .Returns<IEnvelope>((arg) => arg);
            mockedIBoard
               .Setup(board => board.DELETE(It.IsAny<IEnvelope>()))
               .Returns<IEnvelope>((arg) => arg);
            return mockedIBoard.Object;
        }

        private IBoards GetMockedIBoards()
        {
            var mockedIBoards = new Mock<IBoards>();
            mockedIBoards
                .Setup(boards => boards.InitializeBoard_SQLDataBaseBoardChatMessage())
                .Returns(true);
            mockedIBoards
                .Setup(boards => boards.GetHandle_SQLDataBaseBoardChatMessage())
                .Returns(GetMockedISQLDataBaseBoardChatMessage());
            return mockedIBoards.Object;
        }

        [TestMethod]
        public void TestITackCRUD()
        {
            IBoards boards = GetMockedIBoards(); 
            IEnvelope envelope = _erector.Container.Resolve<IEnvelope>();
            envelope.ServiceRoute = "ChatMessage";
            IChatMessageEnvelope chatMessageEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            chatMessageEnvelope.ServiceRoute = "ChatMessage";

            //Boards cannot be null.
            ITack nullBoardsTack = new Tack(null);
            try
            {
                IEnvelope envelopeNullBoard = nullBoardsTack.GET(envelope);
                Assert.Fail();
            }
            catch(InvalidOperationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(message, nullBoardsTack.ExceptionMessage_BoardsCannotBeNull);
            }

            ITack tack = new Tack(boards);

            //Create
            IEnvelope createEnvelope = tack.POST(envelope);
            Assert.IsNotNull(createEnvelope);
            Assert.AreEqual(createEnvelope.ServiceRoute, envelope.ServiceRoute);

            IEnvelope createChatMessageEnvelope = tack.POST(chatMessageEnvelope);
            Assert.IsNotNull(createChatMessageEnvelope);
            Assert.AreEqual(createChatMessageEnvelope.ServiceRoute, envelope.ServiceRoute);
            Assert.IsTrue(createChatMessageEnvelope.GetType() == typeof(ChatMessageEnvelope));

            //Read
            IEnvelope readEnvelope = tack.GET(envelope);
            Assert.IsNotNull(readEnvelope);
            Assert.AreEqual(readEnvelope.ServiceRoute, envelope.ServiceRoute);

            IEnvelope readChatMessageEnvelope = tack.GET(chatMessageEnvelope);
            Assert.IsNotNull(readChatMessageEnvelope);
            Assert.AreEqual(readChatMessageEnvelope.ServiceRoute, envelope.ServiceRoute);
            Assert.IsTrue(readChatMessageEnvelope.GetType() == typeof(ChatMessageEnvelope));

            //Update
            IEnvelope updateEnvelope = tack.PUT(envelope);
            Assert.IsNotNull(updateEnvelope);
            Assert.AreEqual(updateEnvelope.ServiceRoute, envelope.ServiceRoute);

            IEnvelope updateChatMessageEnvelope = tack.PUT(chatMessageEnvelope);
            Assert.IsNotNull(updateChatMessageEnvelope);
            Assert.AreEqual(updateChatMessageEnvelope.ServiceRoute, envelope.ServiceRoute);
            Assert.IsTrue(updateChatMessageEnvelope.GetType() == typeof(ChatMessageEnvelope));

            //Delete
            IEnvelope deleteEnvelope = tack.DELETE(envelope);
            Assert.IsNotNull(deleteEnvelope);
            Assert.AreEqual(deleteEnvelope.ServiceRoute, envelope.ServiceRoute);

            IEnvelope deleteChatMessageEnvelope = tack.DELETE(chatMessageEnvelope);
            Assert.IsNotNull(deleteChatMessageEnvelope);
            Assert.AreEqual(deleteChatMessageEnvelope.ServiceRoute, envelope.ServiceRoute);
            Assert.IsTrue(deleteChatMessageEnvelope.GetType() == typeof(ChatMessageEnvelope));

        }
    }
}
