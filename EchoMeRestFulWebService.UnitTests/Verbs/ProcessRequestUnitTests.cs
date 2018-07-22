using System;
using EchoMeRestFulWebService.Interface;
using EchoMeRestFulWebService.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EchoMeRestFulWebService.UnitTests.Verbs
{
    [TestClass]
    public class ProcessRequestUnitTests
    {
        IEchoMeService _echoMe { get; set; }

        public ProcessRequestUnitTests()
        {
            _echoMe = new EchoMeService();
        }

        [TestMethod]
        public void Test_HEAD_GET_DELETE()
        {
            string message = "Jesus Loves You.";
            string url = String.Format(@"http://EchoMeRestFulWebService?message={0}", message);
            string response = String.Empty;

            //HEAD
            

            //GET

            //DELETE


        }

        [TestMethod]
        public void Test_POST_PUT()
        {
            //POST

            //PUT
        }
    }
}
