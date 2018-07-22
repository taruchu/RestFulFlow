using System;
using System.Web;

namespace EchoMeRestFulWebService.Interface
{
    public interface IEchoMeService : IHttpHandler, IDisposable
    {
        /*
         * HEAD - http://EchoMeRestFulWebService?message=yourmessagehere
         * 
         * GET - http://EchoMeRestFulWebService?message=yourmessagehere
         *  
         * DELETE - http://EchoMeRestFulWebService?message=yourmessagehere
         * 
         * PUT - http://EchoMeRestFulWebService
         * 
         * POST - http://EchoMeRestFulWebService
         * 
         */

        void Echo(byte[] request, HttpContext context);
    }
}
