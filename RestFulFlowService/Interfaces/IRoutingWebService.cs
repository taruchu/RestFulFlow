using Microsoft.AspNetCore.Http;
using SharedInterfaces.Interfaces.Proxy;
using SharedInterfaces.Interfaces.ServiceFarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestFulFlowService.Interfaces
{
    public interface IRoutingWebService : IDisposable
    {
        /*
         * Client calling this web service only needs to provide the valid json envelope in the request body
         * for the service they need, and use the proper web method.
         * 
         */


        /// <summary>
        /// GET - Routing Service Message bus receives a get request envelope for some service.
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Get(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy);

        /// <summary>
        /// PUT - Routing Service Message Bus recieves a request for a service to update something 
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Put(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy);

        /// <summary>
        /// POST - Routing Service Message Bus receives a request for another service to create something or it's simply a response for another service to pickup.
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Post(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy);

        /// <summary>
        /// DELETE - Routing Service Message Bus receives a request for another service to delete something.
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Delete(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy);

        /// <summary>
        ///  HEAD - Get request without payload
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Head(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy);

        /// <summary>
        /// Process the json string inside of the service farm and return a json response.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="serviceFarmLoadBalancer"></param>
        /// <param name="clientProxy"></param>
        /// <returns></returns>
        string ProcessRequestInServiceFarm(string json, IServiceFarmLoadBalancer serviceFarmLoadBalancer, IClientProxy clientProxy);

    }
}
