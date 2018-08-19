using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public enum StorageMechanisms { JSON_FILE = 0, NOSQL_XYZ = 1, SQL_DBase_ABC = 2, IN_MEMORY_CACHE = 3 };

    public interface IBoard
    {
        /*
         * This interface will manage the availiable data storage types.
         * It will instantiate/initialize all storage mechanisms and provide access to them via a table.
         * 
         */

        //IDataInMemoryCache<IEnvelope>
    }
}
