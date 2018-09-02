﻿using DataPersistence.Interfaces;
using DataPersistence.Interfaces.Configuration;
using Newtonsoft.Json;
using SharedUtilities.Implementation.Marshall;
using SharedUtilities.Interfaces.Marshall;
using System;
using System.IO;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace DataPersistence.Services.Configuration
{
    public class SQLDBConfigurationProvider : ISQLDBConfigurationProvider
    {
        private string _CONFIGURATION_FILE_NAME = "SQLDBConfiguration.json"; 
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
                        _sQLDBConfiguration = (SQLDBConfiguration)serializer.Deserialize(file, typeof(SQLDBConfiguration));
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
                    _sQLDBConfiguration = (SQLDBConfiguration)JsonConvert.DeserializeObject<SQLDBConfiguration>(json);
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
