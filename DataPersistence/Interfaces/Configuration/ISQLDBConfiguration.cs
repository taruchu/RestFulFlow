using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public interface ISQLDBConfiguration
    {
        string ConnectionString { get; set; }
    }
}
