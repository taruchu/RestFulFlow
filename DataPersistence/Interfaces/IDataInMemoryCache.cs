using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public interface IDataInMemoryCache<T> : IDisposable
    {
        //TODO: This needs a threshold with eviction policy
        T GET(long ID);
        bool PUT(long ID, T data);
        bool POST(long ID, T data);
        bool DELETE(long ID);

        string ExceptionMessage_DataCannotBeNull { get; }
        string ExceptionMessage_IDCannotBeNegative { get; }
    }
}
