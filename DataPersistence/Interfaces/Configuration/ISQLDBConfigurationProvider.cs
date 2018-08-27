using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces.Configuration
{
    public interface ISQLDBConfigurationProvider
    {
        ISQLDBConfiguration GetSQLDBConfigurationFromJSONFile();
        ISQLDBConfiguration GetSQLDBConfigurationFromJSONString(string json);
    }
}
