using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataPersistence.Services.IOC;
using DataPersistence.Interfaces.Configuration;
using DataPersistence.Interfaces;

namespace DataPersistence.UnitTests.Configuration
{
    [TestClass]
    public class SQLDBConfigurationProviderUnitTests
    {
        private ErectDIContainer _erector { get; set; }
        public SQLDBConfigurationProviderUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        [TestMethod]
        public void TestGetSQLDBConfigurationFromJSONFile()
        {
            ISQLDBConfigurationProvider sQLDBConfigurationProvider = _erector.Container.Resolve<ISQLDBConfigurationProvider>();
            ISQLDBConfiguration sQLDBConfiguration = sQLDBConfigurationProvider.GetSQLDBConfigurationFromJSONFile();
            Assert.IsNotNull(sQLDBConfiguration);
            Assert.IsFalse(String.IsNullOrEmpty(sQLDBConfiguration.ConnectionString));
        }

        [TestMethod]
        public void TestGetSQLDBConfigurationFromJSONString()
        {
            ISQLDBConfigurationProvider sQLDBConfigurationProvider = _erector.Container.Resolve<ISQLDBConfigurationProvider>();
            string jsonConfig = "{'ConnectionString': 'Server=JESUS;Database=ChatMessageDB;Trusted_Connection=True;'}";
            ISQLDBConfiguration sQLDBConfiguration = sQLDBConfigurationProvider.GetSQLDBConfigurationFromJSONString(jsonConfig);
            Assert.IsNotNull(sQLDBConfiguration);
            Assert.IsFalse(String.IsNullOrEmpty(sQLDBConfiguration.ConnectionString));
        }
    }
}
