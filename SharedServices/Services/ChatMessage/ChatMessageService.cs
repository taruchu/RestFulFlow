﻿using System;
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
        public IMessageBusBank<string> MessageBusBank { get; set; }
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

        public string ExceptionMessage_MessageBusWriterCannotBeNull
        {
            get
            {
                return "ChatMessageService - MessageBusWriter cannot be null.";
            }
        }
        public string ExceptionMessage_MessageBusReaderBankCannotBeNull
        {
            get
            {
                return "ChatMessageService - MessageBusReaderBank cannot be null.";
            }
        }

        public string ExceptionMessage_MessageBusBankCannotBeNull
        {
            get
            {
                return "ChatMessageService - MessageBusBank cannot be null.";
            }
        }

        public string ExceptionMessage_MarshallerCannotBeNull
        {
            get
            {
                return "ChatMessageService - Marshaller cannot be null.";
            }
        }

        public ChatMessageService()
        {
            _isDisposed = false;
            HandleMessageFromRouter = ProcessMessage;
        }


        public void ProcessMessage(string message)
        {
            try
            {
                //TODO: For now just echo it back to the sender. Later add hooks to the GET, POST, PUT, DELETE methods.
                //TODO: I want to move this chat message service into a WebSocket entry point instead of a RestFul entry point.

                IChatMessageEnvelope envelope = Marshaller.UnMarshall<IChatMessageEnvelope>(message);
                string ClientProxyGUID = envelope.ClientProxyGUID;   
                SendResponse(ClientProxyGUID, message);
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
                    _isDisposed = true;
                }
            }
            catch (Exception ex)
            {
                new ApplicationException(ex.Message, ex);
            }
        }

        public bool SendResponse(string ClientProxyGUID, string responseBody)
        {
            try
            {
               return MessageBusBank.ResolveMessageBus(ClientProxyGUID).SendMessage(responseBody);                
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public string GetNewest(IChatMessageEnvelope request)
        {
            throw new NotImplementedException();
        }

        public string Put(IChatMessageEnvelope request)
        {
            throw new NotImplementedException();
        }

        public string Post(IChatMessageEnvelope request)
        {
            throw new NotImplementedException();
        }

        public string Delete(IChatMessageEnvelope request)
        {
            throw new NotImplementedException();
        }
    }
}
