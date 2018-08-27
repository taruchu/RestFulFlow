using ChatMessageInterfaces.Interfaces.ChatMessage;
using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Routing;
using SharedUtilities.Interfaces.Marshall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Services.ChatMessage
{
    public class GetNextChatMessageService : IGetNextChatMessageService
    {
        public string ServiceName { get; set; }
        public IMessageBusWriter<string> MessageBusWiter { get; set; }
        public IMessageBusReaderBank<string> MessageBusReaderBank { get; set; }
        public Action<string> HandleMessageFromRouter { get; set; }
        public IMessageBusBank<string> MessageBusBank { get; set; }
        public ITack Tack { get; set; }
        private IMarshaller _marshaller { get; set; }
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

        public string ExceptionMessage_MessageBusWriterCannotBeNull
        {
            get
            {
                return "GetNextChatMessageService - MessageBusWriter cannot be null.";
            }
        }
        public string ExceptionMessage_MessageBusReaderBankCannotBeNull
        {
            get
            {
                return "GetNextChatMessageService - MessageBusReaderBank cannot be null.";
            }
        }

        public string ExceptionMessage_MessageBusBankCannotBeNull
        {
            get
            {
                return "GetNextChatMessageService - MessageBusBank cannot be null.";
            }
        }

        public string ExceptionMessage_MarshallerCannotBeNull
        {
            get
            {
                return "GetNextChatMessageService - Marshaller cannot be null.";
            }
        }

        public string ExceptionMessage_ITackCannotBeNull
        {
            get
            {
                return "ModifyChatMessageService - ITack cannot be null.";
            }
        }

        public GetNextChatMessageService(IMarshaller marshaller)
        {
            _isDisposed = false;
            HandleMessageFromRouter = ProcessMessage;
            _marshaller = marshaller; 
        }


        public void ProcessMessage(string message)
        {
            try
            {
                if (_marshaller == null)
                    throw new InvalidOperationException(ExceptionMessage_MarshallerCannotBeNull);
                else
                { 
                    IChatMessageEnvelope requestEnvelope = _marshaller.UnMarshall<IChatMessageEnvelope>(message);
                    string responseEnvelope = Get(requestEnvelope);
                    string ClientProxyGUID = requestEnvelope.ClientProxyGUID;
                    SendResponse(ClientProxyGUID, responseEnvelope);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                new ApplicationException(ex.Message, ex);
            }
        }

        public void Dispose()
        {
            try
            {
                if (_isDisposed == false)
                {
                    MessageBusWiter.Dispose();
                    MessageBusReaderBank.Dispose();
                    Tack.Dispose();
                    _isDisposed = true;
                }
            }
            catch (Exception ex)
            {
                new ApplicationException(ex.Message, ex);
            }
        }

        public string Get(IChatMessageEnvelope request)
        {
            try
            {
                if (Tack == null)
                    throw new InvalidOperationException(ExceptionMessage_ITackCannotBeNull);
                else
                {
                    //NOTE: Define the query
                    request.Query =
                        (chatMessageQueryRepo, chatMessageEnvelopeParam) =>
                        {
                            IChatMessageEnvelope nextChatMessage = chatMessageQueryRepo.GetNextChatMessage(chatMessageEnvelopeParam);
                            return nextChatMessage;
                        };
                         
                    IChatMessageEnvelope responseEnvelope = (IChatMessageEnvelope)Tack.GET(request);
                    string responseString = _marshaller.MarshallPayloadJSON(responseEnvelope);
                    return responseString;
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public bool SendResponse(string ClientProxyGUID, string responseBody)
        {
            try
            {
                if (MessageBusBank == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusBankCannotBeNull);

                return MessageBusBank.ResolveMessageBus(ClientProxyGUID).SendMessage(responseBody);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
