using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public enum StorageMechanisms { FILE_DB = 0, NOSQL_XYZ = 1, SQL_DBase_ABC = 2, IN_MEMORY_CACHE = 3 };

    public interface IBoard : IDisposable
    {
        /*
         * This interface will manage the availiable data storage types.
         * It will instantiate/initialize all storage mechanisms and provide access to them via a table.
         * 
         */

        bool InitializeAllBoards(); 
        IDataInMemoryCache<IEnvelope> GetHandle_DataInMemoryCache();
        IFileStorage GetHandle_FileStorage();
    }
}
