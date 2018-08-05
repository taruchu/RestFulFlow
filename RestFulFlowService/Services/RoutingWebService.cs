using Microsoft.AspNetCore.Http;
using RestFulFlowService.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            switch (context.Request.Method)
            {
                case "GET":
                    responsePayload = Get(requestPayload, context);
                    break;
                case "PUT":
                    responsePayload = Put(requestPayload, context);
                    break;
                case "POST":
                    responsePayload = Post(requestPayload, context);
                    break;
                case "DELETE":
                    responsePayload = Delete(requestPayload, context);
                    break;
                case "HEAD":
                    responsePayload = Head(requestPayload, context);
                    break;
                default:
                    break;
            }

            await context.Response.WriteAsync(responsePayload);
        }

        public string Delete(string json, HttpContext context)
        {
            //NOTE:
            // Salt the request header key values with DELETE and client origin source 

            return json;
        }

        public void Dispose()
        { 
        }

        public string Get(string json, HttpContext context)
        {
            //NOTE:
            // Salt the request header key values with GET and client origin source 


            //TODO:
            // (1) Get transient ClientProxy and singleton IServiceFarmLoadBalancer from the 
            // IOC container using the HttpContext.

            // (2) Pass json requestBody and ClientProxy responseCallback into IServiceFarmLoadBalancer request method
            // (3) Poll the ClientProxy message bus using one of the ClientProxies method calls
            // (4) When the ClientProxy message bus receives a response, the method will return it as a string
            // (5) Save the response from the ClientProxy
            // (6) Send a Release command to the IServiceFarmLoadBalancer so that the ClientProxies requestCallback is removed from the router
            // (7) Return the saved response from the ClientProxy

            return json;
        }

        public string Head(string json, HttpContext context)
        {
            //NOTE:
            // Salt the request header key values with HEAD and client origin source 

            return json;
        }

        public string Post(string json, HttpContext context)
        {
            //NOTE:
            // Salt the request header key values with POST and client origin source 

            return json;
        }

        public string Put(string json, HttpContext context)
        {
            //NOTE:
            // Salt the request header key values with PUT and client origin source 

            return json;
        }
    }
}
