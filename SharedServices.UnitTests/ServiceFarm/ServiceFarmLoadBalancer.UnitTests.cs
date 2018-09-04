using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Proxy;
using SharedInterfaces.Interfaces.ServiceFarm;
using SharedServices.Models.Constants;
using SharedServices.Services.IOC;
using SharedUtilities.Interfaces.Marshall;
using System;

namespace SharedServices.UnitTests.ServiceFarm
{
    [TestClass]
    public class ServiceFarmLoadBalancerUnitTests
    {
        private ErectDIContainer _erector { get; set; }

        public ServiceFarmLoadBalancerUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        private IChatMessageEnvelope GetValidChatMessageEnvelope()
        {
            IChatMessageEnvelope chatMessageEnvelope = _erector.Container.Resolve<IChatMessageEnvelope>();
            chatMessageEnvelope.ServiceRoute = ChatServiceNames.GetNextChatMessageService;
            chatMessageEnvelope.ClientProxyGUID = Guid.NewGuid().ToString();
            chatMessageEnvelope.RequestMethod = "GET";
            chatMessageEnvelope.ChatMessageID = 0;
            chatMessageEnvelope.ChatChannelID = 1;
            chatMessageEnvelope.ChatChannelName = "AwesomeSoft";
            chatMessageEnvelope.SenderUserName = "Jesus";
            chatMessageEnvelope.ChatMessageBody = "I love you Dionn.";
            chatMessageEnvelope.CreatedDateTime = DateTime.MinValue;
            chatMessageEnvelope.ModifiedDateTime = DateTime.MinValue;
            return chatMessageEnvelope;
        }

        [TestMethod]
        public void TestServiceFarmLoadBalancerSendServiceRequest()
        {
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IChatMessageEnvelope requestEnvelope = GetValidChatMessageEnvelope();
            string requestPayload = marshaller.MarshallPayloadJSON(requestEnvelope);
            string clientProxyOriginGUID = Guid.NewGuid().ToString();

            bool success = serviceFarmLoadBalancer.SendServiceRequest(clientProxyOriginGUID, requestPayload);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void TestServiceFarmLoadBalancerRegisterClientProxyMessageBus()
        {
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();

            bool success = serviceFarmLoadBalancer.RegisterClientProxyMessageBus(clientProxy);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void TestServiceFarmLoadBalancerReleaseClientProxyMessageBus()
        {
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();

            bool success = serviceFarmLoadBalancer.RegisterClientProxyMessageBus(clientProxy);
            Assert.IsTrue(success);
            success = false;

            success = serviceFarmLoadBalancer.ReleaseClientProxyMessageBus(clientProxy);
            Assert.IsTrue(success);
        }
    }
}
