using DataPersistence.Interfaces.Files;
using DataPersistence.Interfaces.SQL;
using SharedInterfaces.Interfaces.Envelope;
using System;

namespace DataPersistence.Interfaces
{
    public interface IBoards : IDisposable
    {
        /*
         * This interface will manage the availiable data storage types.
         * It will instantiate/initialize those types and provide access to them via a handle.
         * 
         */
        
        bool InitializeBoard_SQLDataBaseBoardChatMessage(); 
        IDataInMemoryCache<IEnvelope> GetHandle_DataInMemoryCache();
        IFileStorage GetHandle_FileStorage();
        ISQLDataBaseBoardChatMessage GetHandle_SQLDataBaseBoardChatMessage();
    }
}
