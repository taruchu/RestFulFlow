using Microsoft.AspNetCore.Http;
using RestFulFlowService.Interfaces;
using SharedInterfaces.Interfaces.Proxy;
using SharedInterfaces.Interfaces.ServiceFarm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestFulFlowService.Services
{
    public class RoutingWebService : IRoutingWebService
    {
        private string _contentTypeJSON { get { return "application/json"; } }
        

        public RoutingWebService(RequestDelegate next)
        {
            
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.ContentType != _contentTypeJSON)
                return;

            context.Response.ContentType = _contentTypeJSON;
            string requestPayload = String.Empty;
            string responsePayload = String.Empty;

            using (var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8))
            {
                requestPayload = reader.ReadToEnd();
            }

            using (IServiceFarmLoadBalancer serviceFarmLoadBalancer = (IServiceFarmLoadBalancer)context.RequestServices.GetService(typeof(IServiceFarmLoadBalancer)))
            {
                using (IClientProxy clientProxy = (IClientProxy)context.RequestServices.GetService(typeof(IClientProxy)))
                {
                    switch (context.Request.Method)
                    {
                        case "GET":
                            responsePayload = Get(requestPayload, serviceFarmLoadBalancer, clientProxy);
                            break;
                        case "PUT":
                            responsePayload = Put(requestPayload, serviceFarmLoadBalancer, clientProxy);
                            break;
                        case "POST":
                            responsePayload = Post(requestPayload, serviceFarmLoadBalancer, clientProxy);
                            break;
                        case "DELETE":
                            responsePayload = Delete(requestPayload, serviceFarmLoadBalancer, clientProxy);
                            break;
                        case "HEAD":
                            responsePayload = Head(requestPayload, serviceFarmLoadBalancer, clientProxy);
                            context.Response.ContentType = _contentTypeJSON;
                            context.Response.ContentLength = System.Text.Encoding.UTF8.GetBytes(responsePayload).LongLength;
                            return;
                        default:
                            break;
                    }
                }
            }

            context.Response.ContentType = _contentTypeJSON;
            await context.Response.WriteAsync(responsePayload);
        }

        public string Delete(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy)
        { 
            //NOTE: Can do other processing of the request before and or after this call.
            return ProcessRequestInServiceFarm(json, serviceFarmLoadBalancer, clientProxy);
        }

        public string ProcessRequestInServiceFarm(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy)
        { 
            serviceFarmLoadBalancer.RegisterClientProxyMessageBus(clientProxy);
            string response = String.Empty;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            if (serviceFarmLoadBalancer.SendServiceRequest(clientProxy.ServiceGUID, json))
                response = clientProxy.PollMessageBus(cancellationTokenSource);

            serviceFarmLoadBalancer.ReleaseClientProxyMessageBus(clientProxy);
            return response;
        }

        public void Dispose()
        { 
        }

        public string Get(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy)
        {
            //NOTE: Can do other processing of the request before and or after this call.
            return ProcessRequestInServiceFarm(json, serviceFarmLoadBalancer, clientProxy);
        }

        public string Head(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy)
        {
            //NOTE: Can do other processing of the request before and or after this call.
            return ProcessRequestInServiceFarm(json, serviceFarmLoadBalancer, clientProxy); 
        }

        public string Post(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy)
        {
            //NOTE: Can do other processing of the request before and or after this call.
            return ProcessRequestInServiceFarm(json, serviceFarmLoadBalancer, clientProxy); 
        }

        public string Put(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy)
        {
            //NOTE: Can do other processing of the request before and or after this call.
            return ProcessRequestInServiceFarm(json, serviceFarmLoadBalancer, clientProxy); 
        }
    }
}
