using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedServices.Services.IOC;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestFulFlowService.UnitTests.WebServices
{
    [TestClass]
    class ServiceFarmHubUnitTests
    {
        private ErectDIContainer _erector { get; set; }
        public ServiceFarmHubUnitTests()
        {
            _erector = new ErectDIContainer();
        }

    }
}
