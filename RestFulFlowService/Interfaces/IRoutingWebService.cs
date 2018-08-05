using Microsoft.AspNetCore.Http;
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
        string Get(string json, HttpContext context);

        /// <summary>
        /// PUT - Routing Service Message Bus recieves a request for a service to update something 
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Put(string json, HttpContext context);

        /// <summary>
        /// POST - Routing Service Message Bus receives a request for another service to create something or it's simply a response for another service to pickup.
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Post(string json, HttpContext context);

        /// <summary>
        /// DELETE - Routing Service Message Bus receives a request for another service to delete something.
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Delete(string json, HttpContext context);

        /// <summary>
        ///  HEAD - Get request without payload
        /// </summary>
        /// <param name="json">json request envelope</param>
        /// <param name="context">HttpContext</param>
        /// <returns>json response envelope</returns>
        string Head(string json, HttpContext context);

    }
}
