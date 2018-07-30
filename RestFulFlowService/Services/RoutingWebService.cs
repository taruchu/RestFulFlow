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
            //TODO: Initialize SharedServices Entry Point here using a private member.
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
                    responsePayload = Get(requestPayload);
                    break;
                case "PUT":
                    responsePayload = Put(requestPayload);
                    break;
                case "POST":
                    responsePayload = Post(requestPayload);
                    break;
                case "DELETE":
                    responsePayload = Delete(requestPayload);
                    break;
                case "HEAD":
                    responsePayload = Head(requestPayload);
                    break;
                default:
                    break;
            }

            await context.Response.WriteAsync(responsePayload);
        }

        public string Delete(string json)
        {
            //TODO: Get Client Proxy from SharedServices entry point.
            //TODO: Pass json parameter into client proxy and wait for json response.
            //TODO: Return json response from client proxy.
            return json;
        }

        public void Dispose()
        { 
        }

        public string Get(string json)
        {
            //TODO: Get Client Proxy from SharedServices entry point.
            //TODO: Pass json parameter into client proxy and wait for json response.
            //TODO: Return json response from client proxy.
            return json;
        }

        public string Head(string json)
        {
            //TODO: Get Client Proxy from SharedServices entry point.
            //TODO: Pass json parameter into client proxy and wait for json response.
            //TODO: Return json response from client proxy.
            return json;
        }

        public string Post(string json)
        {
            //TODO: Get Client Proxy from SharedServices entry point.
            //TODO: Pass json parameter into client proxy and wait for json response.
            //TODO: Return json response from client proxy.
            return json;
        }

        public string Put(string json)
        {
            //TODO: Get Client Proxy from SharedServices entry point.
            //TODO: Pass json parameter into client proxy and wait for json response.
            //TODO: Return json response from client proxy.
            return json;
        }
    }
}
