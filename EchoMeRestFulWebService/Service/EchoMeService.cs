using EchoMeRestFulWebService.Interface;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace EchoMeRestFulWebService.Service
{
    public class EchoMeService : IEchoMeService
    {
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void Dispose()
        { 
        }

        public void Echo(byte[] request, HttpContext context)
        {  
            context.Response.ContentType = context.Request.ContentType;
            context.Response.BinaryWrite(request);
        }
         
        private byte[] ReadRequestBody(Stream body)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                body.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public void ProcessRequest(HttpContext context)
        { 
            byte[] request = null;

            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                case "DELETE":
                    request = Encoding.UTF8.GetBytes(context.Request["message"]);
                    Echo(request, context);
                    break; 
                case "POST":
                case "PUT": 
                    byte[] body = ReadRequestBody(context.Request.InputStream);
                    Echo(body, context);
                    break; 
                default:
                    break;
            }
        }
    }
}
