using DataPersistence.Interfaces;
using DataPersistence.Interfaces.Files;
using DataPersistence.Interfaces.SQL;
using DataPersistence.Services.IOC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedInterfaces.Interfaces.Envelope;

namespace DataPersistence.UnitTests
{
    [TestClass]
    public class BoardsUnitTests
    {
        private ErectDIContainer _erector { get; set; }
        public BoardsUnitTests()
        {
            _erector = new ErectDIContainer();
        }
         

        [TestMethod]
        public void TestInteractionWithSQLDataBaseBoardChatMessage()
        {
            IBoards boards = _erector.Container.Resolve<IBoards>();
            bool success = boards.InitializeBoard_SQLDataBaseBoardChatMessage();
            Assert.IsTrue(success);

            ISQLDataBaseBoardChatMessage sQLDataBaseBoardChatMessage = boards.GetHandle_SQLDataBaseBoardChatMessage();
            Assert.IsNotNull(sQLDataBaseBoardChatMessage);
        }

        [TestMethod]
        public void TestFileStorage()
        {
            IBoards boards = _erector.Container.Resolve<IBoards>();
            IFileStorage fileStorage = boards.GetHandle_FileStorage();
            Assert.IsNotNull(fileStorage);
        }

        [TestMethod]
        public void TestIDataInMemeoryCache()
        {
            IBoards boards = _erector.Container.Resolve<IBoards>();
            IDataInMemoryCache<IEnvelope> dataCache = boards.GetHandle_DataInMemoryCache();
            Assert.IsNotNull(dataCache);
        }

    }
}
