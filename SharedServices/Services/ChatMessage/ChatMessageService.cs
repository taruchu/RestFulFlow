using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SharedServices.Interfaces.ChatMessage;
using SharedServices.Interfaces.Envelope;
using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.Routing;
using SharedServices.Models.Constants;
using SharedServices.Services.Routing;

namespace SharedServices.Services.ChatMessage
{
    public class ChatMessageService : IChatMessageService
    {
        public string ServiceName { get; set; }
        public IMessageBusWriter<string> MessageBusWiter { get; set; }
        public IMessageBusReaderBank<string> MessageBusReaderBank { get; set; }
        public Action<string> HandleMessageFromRouter { get; set; }
        public IMarshaller Marshaller { get; set; }
        public string ServiceGUID
        {
            get
            {
                if (String.IsNullOrEmpty(_serviceGUID))
                    _serviceGUID = Guid.NewGuid().ToString();
                return _serviceGUID;
            }
        }
        private string _serviceGUID { get; set; }
        private bool _isDisposed { get; set; }
        
        public ChatMessageService()
        {
            _isDisposed = false;
            HandleMessageFromRouter = ProcessMessage;
        }


        public void ProcessMessage(string message)
        {
            //TODO: For now just echo it back to the sender. Later add all the proper mechanics.
            IEnvelope envelope = Marshaller.UnMarshall(message);
            string WebServiceOriginUrl = envelope.Header_KeyValues[JSONSchemas.WebServiceOriginUrl];
            string clientProxyOrigin = envelope.Header_KeyValues[JSONSchemas.ClientProxyOrigin]; 
            string destinationRoute = envelope.Header_KeyValues[JSONSchemas.DestinationRoute];
            string destinationRouter = destinationRoute.Split('.')[0];
            envelope.Header_KeyValues[JSONSchemas.DestinationRoute] = String.Format("{0}.{1}", destinationRouter, clientProxyOrigin);
            envelope.Header_KeyValues[JSONSchemas.RequestMethod] = "POST";            
            envelope.Header_KeyValues[JSONSchemas.SenderRoute] = destinationRoute;
            string postRequest = Marshaller.MarshallPayloadJSON(envelope);
            Post(WebServiceOriginUrl, postRequest);
        }

        public void Dispose()
        {
            if(_isDisposed == false)
            {
                MessageBusWiter.Dispose();
                MessageBusReaderBank.Dispose();
                _isDisposed = true;
            }
        }

        public bool Post(string serviceUrl, string responseBody)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(serviceUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> responseTask = client.GetAsync(serviceUrl);
                responseTask.Wait();
                switch (responseTask.Status)
                {
                    case TaskStatus.RanToCompletion:
                        client.Dispose();
                        return responseTask.Result.IsSuccessStatusCode;
                    case TaskStatus.Faulted:
                        throw new ApplicationException(responseTask.Exception.Flatten().InnerException.Message, responseTask.Exception.Flatten().InnerException);
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
