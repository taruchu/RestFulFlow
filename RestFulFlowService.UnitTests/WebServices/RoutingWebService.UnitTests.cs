
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFulFlowService.Interfaces;
using RestFulFlowService.Services;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Proxy;
using SharedInterfaces.Interfaces.ServiceFarm;
using SharedServices.Models.Constants;
using SharedServices.Services.IOC;
using SharedUtilities.Interfaces.Marshall;
using System;

namespace RestFulFlowService.UnitTests.WebServices
{
    [TestClass]
    public class RoutingWebServiceUnitTests
    {
        private ErectDIContainer _erector { get; set; } 

        public RoutingWebServiceUnitTests()
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
        public void TestRoutingWebServiceProcessRequestInServiceFarm()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();
            IRoutingWebService routingWebService = new RoutingWebService(default(RequestDelegate)); 
            IChatMessageEnvelope chatMessageEnvelope = GetValidChatMessageEnvelope();
            chatMessageEnvelope.ServiceRoute = ChatServiceNames.GetNextChatMessageService;
           
            string requestEnvelope = marshaller.MarshallPayloadJSON(chatMessageEnvelope);
            string responseEnvelope = routingWebService.ProcessRequestInServiceFarm(requestEnvelope, serviceFarmLoadBalancer, clientProxy);

            Assert.IsFalse(String.IsNullOrEmpty(responseEnvelope)); 
        }

        [TestMethod]
        public void TestRoutingWebServiceGET()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();
            IRoutingWebService routingWebService = new RoutingWebService(default(RequestDelegate));
            IChatMessageEnvelope chatMessageEnvelope = GetValidChatMessageEnvelope();
            chatMessageEnvelope.ServiceRoute = ChatServiceNames.GetNextChatMessageService;

            string requestEnvelope = marshaller.MarshallPayloadJSON(chatMessageEnvelope);
            string responseEnvelope = routingWebService.Get(requestEnvelope, serviceFarmLoadBalancer, clientProxy);

            Assert.IsFalse(String.IsNullOrEmpty(responseEnvelope)); 
        }

        [TestMethod]
        public void TestRoutingWebServicePUT()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();
            IRoutingWebService routingWebService = new RoutingWebService(default(RequestDelegate));
            IChatMessageEnvelope chatMessageEnvelope = GetValidChatMessageEnvelope();
            chatMessageEnvelope.ServiceRoute = ChatServiceNames.ModifyChatMessageService;
            chatMessageEnvelope.RequestMethod = "PUT";
            chatMessageEnvelope.ChatMessageID = 18;
            chatMessageEnvelope.ChatMessageBody = "God is love";


            string requestEnvelope = marshaller.MarshallPayloadJSON(chatMessageEnvelope);
            string responseEnvelope = routingWebService.Put(requestEnvelope, serviceFarmLoadBalancer, clientProxy);
             
            Assert.IsFalse(String.IsNullOrEmpty(responseEnvelope));
        }

        [TestMethod]
        public void TestRoutingWebServicePOST()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();
            IRoutingWebService routingWebService = new RoutingWebService(default(RequestDelegate));
            IChatMessageEnvelope chatMessageEnvelope = GetValidChatMessageEnvelope();
            chatMessageEnvelope.ServiceRoute = ChatServiceNames.ModifyChatMessageService;
            chatMessageEnvelope.RequestMethod = "POST";
            chatMessageEnvelope.ChatMessageID = 0;
            
            string requestEnvelope = marshaller.MarshallPayloadJSON(chatMessageEnvelope);
            string responseEnvelope = routingWebService.Post(requestEnvelope, serviceFarmLoadBalancer, clientProxy);

            Assert.IsFalse(String.IsNullOrEmpty(responseEnvelope));
        }

        [TestMethod]
        public void TestRoutingWebServiceDELETE()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();
            IRoutingWebService routingWebService = new RoutingWebService(default(RequestDelegate));
            IChatMessageEnvelope chatMessageEnvelope = GetValidChatMessageEnvelope();
            chatMessageEnvelope.ServiceRoute = ChatServiceNames.ModifyChatMessageService;
            chatMessageEnvelope.RequestMethod = "DELETE";
            chatMessageEnvelope.ChatMessageID = 19;
            
            string requestEnvelope = marshaller.MarshallPayloadJSON(chatMessageEnvelope);
            string responseEnvelope = routingWebService.Delete(requestEnvelope, serviceFarmLoadBalancer, clientProxy);

            Assert.IsFalse(String.IsNullOrEmpty(responseEnvelope));
        }

        [TestMethod]
        public void TestRoutingWebServiceHEAD()
        {
            IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
            IServiceFarmLoadBalancer serviceFarmLoadBalancer = _erector.Container.Resolve<IServiceFarmLoadBalancer>();
            IClientProxy clientProxy = _erector.Container.Resolve<IClientProxy>();
            IRoutingWebService routingWebService = new RoutingWebService(default(RequestDelegate));
            IChatMessageEnvelope chatMessageEnvelope = GetValidChatMessageEnvelope();
            chatMessageEnvelope.ServiceRoute = ChatServiceNames.GetNextChatMessageService;
            chatMessageEnvelope.RequestMethod = "HEAD";
            
            string requestEnvelope = marshaller.MarshallPayloadJSON(chatMessageEnvelope);
            string responseEnvelope = routingWebService.Head(requestEnvelope, serviceFarmLoadBalancer, clientProxy);

            Assert.IsFalse(String.IsNullOrEmpty(responseEnvelope));
        }
    }
}
