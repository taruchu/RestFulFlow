using DataPersistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Services.Configuration
{
    public class SQLDBConfiguration : ISQLDBConfiguration
    {
        public SQLDBConfiguration()
        {
        }

        public string ConnectionString { get; set; }
    }
}
