using DataPersistence.Interfaces;
using DataPersistence.Interfaces.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DataPersistence.Services.Configuration
{
    public class SQLDBConfigurationProvider : ISQLDBConfigurationProvider
    {
        private const string _CONFIGURATION_FILE_NAME = "SQLDBConfiguration.json"; 
        private ISQLDBConfiguration _sQLDBConfiguration { get; set; }
        public SQLDBConfigurationProvider()
        {
        }
        public ISQLDBConfiguration GetSQLDBConfigurationFromJSONFile()
        {
            try
            {
                if (_sQLDBConfiguration == null)
                {
                    using (StreamReader file = File.OpenText(_CONFIGURATION_FILE_NAME))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        _sQLDBConfiguration = (ISQLDBConfiguration)serializer.Deserialize(file, typeof(ISQLDBConfiguration));
                    }
                }
                return _sQLDBConfiguration;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public ISQLDBConfiguration GetSQLDBConfigurationFromJSONString(string json)
        {
            try
            {
                if (_sQLDBConfiguration == null)
                {
                    _sQLDBConfiguration = JsonConvert.DeserializeObject<ISQLDBConfiguration>(json);
                }
                return _sQLDBConfiguration;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
