using DataPersistence.Interfaces.Files;
using DataPersistence.Interfaces.SQL;
using SharedInterfaces.Interfaces.Envelope;
using System;

namespace DataPersistence.Interfaces
{
    public enum StorageMechanisms { FILE_DB = 0, NOSQL_XYZ = 1, SQL_DBase_ABC = 2, IN_MEMORY_CACHE = 3 };

    public interface IBoards : IDisposable
    {
        /*
         * This interface will manage the availiable data storage types.
         * It will instantiate/initialize all storage mechanisms and provide access to them via a table.
         * 
         */
        //TODO: Need a refactor here so that I can initialize the boards I need. Need a flexible architecture.
        //Maybe split this up into a IBoard and IBoards interface ? Then I can derive children of IBoard that
        //provide specific connection details for a specific type of data storage, while the base provides the connection algorithm/structure that
        //all clients use to make a connection and interact with the IBoard /data storage. This may require some 
        //restful style interface on the IBoard that the ITack can forward envelopes to. So I ITack would take in the 
        //envelope, derive it's type and route it to the right IBoard, which would expose a restful interface that can except envolopes
        //of that type.
        bool InitializeBoard_SQLDataBaseBoardChatMessage();
        bool InitializeBoard_DataInMemoryCache();
        bool InitializeBoard_FileStorage();
        IDataInMemoryCache<IEnvelope> GetHandle_DataInMemoryCache();
        IFileStorage GetHandle_FileStorage();
        ISQLDataBaseBoardChatMessage GetHandle_SQLDataBaseBoardChatMessage();
    }
}
