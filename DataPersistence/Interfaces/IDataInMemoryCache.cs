using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public interface IDataInMemoryCache<T> : IDisposable
    {
        T GET(int ID);
        bool PUT(int ID, T data);
        bool POST(int ID, T data);
        bool DELETE(int ID);

        string ExceptionMessage_DataCannotBeNull { get; }
        string ExceptionMessage_IDCannotBeNegative { get; }
    }
}
