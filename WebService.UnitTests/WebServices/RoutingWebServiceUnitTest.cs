using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedInterfaces.Interfaces.Envelope;
using SharedServices.Services.IOC;
using 

namespace SharedServices.UnitTests.WebServices
{
    [TestClass]
    public class RoutingWebServiceUnitTest
    {
        private ErectDIContainer _erector { get; set; }

        public RoutingWebServiceUnitTest()
        {
            _erector = new ErectDIContainer();
        }

        private IChatMessageEnvelope GetValidChatMessageEnvelope()
        {
            IChatMessageEnvelope chatMessageEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            chatMessageEnvelope.ServiceRoute = "ServiceRoute";
            chatMessageEnvelope.ClientProxyGUID = Guid.NewGuid().ToString();
            chatMessageEnvelope.RequestMethod = "GET";
            chatMessageEnvelope.ChatMessageID = 1;
            chatMessageEnvelope.ChatChannelID = 1;
            chatMessageEnvelope.ChatChannelName = "AwesomeSoft";
            chatMessageEnvelope.SenderUserName = "Jesus";
            chatMessageEnvelope.ChatMessageBody = "I love you Dionn.";
            chatMessageEnvelope.CreatedDateTime = DateTime.MinValue;
            chatMessageEnvelope.ModifiedDateTime = DateTime.MinValue;
            return chatMessageEnvelope;
        }

        [TestMethod]
        public void TestGET()
        {
            IRoutingWebService 
        }

        [TestMethod]
        public void TestPUT()
        {

        }

        [TestMethod]
        public void TestPOST()
        {

        }

        [TestMethod]
        public void TestDELETE()
        {

        }

        [TestMethod]
        public void TestHEAD()
        {

        }
    }
}
