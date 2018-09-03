using System;
using ChatMessageInterfaces.Interfaces.ChatMessage;
using SharedInterfaces.Interfaces.Envelope;
using SharedUtilities.Interfaces.Marshall;
using SharedInterfaces.Interfaces.Routing;
using DataPersistence.Interfaces;

namespace SharedServices.Services.ChatMessage
{
    public class ModifyChatMessageService : IModifyChatMessageService
    {
        public string ServiceName { get; set; }
        public IMessageBusWriter<string> MessageBusWiter { get; set; }
        public IMessageBusReaderBank<string> MessageBusReaderBank { get; set; }
        public Action<string> HandleMessageFromRouter { get; set; }
        public IMessageBusBank<string> MessageBusBank { get; set; }
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
        private object _thisLock { get; set; }

        public string ExceptionMessage_MessageBusWriterCannotBeNull
        {
            get
            {
                return "ModifyChatMessageService - MessageBusWriter cannot be null.";
            }
        }
        public string ExceptionMessage_MessageBusReaderBankCannotBeNull
        {
            get
            {
                return "ModifyChatMessageService - MessageBusReaderBank cannot be null.";
            }
        }

        public string ExceptionMessage_MessageBusBankCannotBeNull
        {
            get
            {
                return "ModifyChatMessageService - MessageBusBank cannot be null.";
            }
        }

        public string ExceptionMessage_MarshallerCannotBeNull
        {
            get
            {
                return "ModifyChatMessageService - Marshaller cannot be null.";
            }
        }

        public string ExceptionMessage_ITackCannotBeNull
        {
            get
            {
                return "ModifyChatMessageService - ITack cannot be null.";
            }
        }

        public ITack Tack { get; set; }
       
         
        public ModifyChatMessageService(IMarshaller marshaller)
        {
            _isDisposed = false;
            HandleMessageFromRouter = AddMessageToBus;
            _marshaller = marshaller;
            _thisLock = new object();
        }

        public void AddMessageToBus(string message)
        {
            lock(_thisLock)
            {
                try
                {
                    if (MessageBusWiter == null)
                        throw new InvalidOperationException(ExceptionMessage_MessageBusWriterCannotBeNull);
                    else
                    {
                        MessageBusWiter.Write(message);
                    }
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public void ProcessMessage(string message)
        {
            lock (_thisLock)
            {
                try
                {
                    if (_marshaller == null)
                        throw new InvalidOperationException(ExceptionMessage_MarshallerCannotBeNull);
                    else
                    {
                        IChatMessageEnvelope requestEnvelope = _marshaller.UnMarshall<IChatMessageEnvelope>(message);
                        string responseEnvelope = String.Empty;
                        string ClientProxyGUID = requestEnvelope.ClientProxyGUID;

                        if (requestEnvelope.RequestMethod == "POST")
                        {
                            responseEnvelope = Post(requestEnvelope);
                            SendResponse(ClientProxyGUID, responseEnvelope);
                        }
                        else if(requestEnvelope.RequestMethod == "PUT")
                        {
                            responseEnvelope = Put(requestEnvelope);
                            SendResponse(ClientProxyGUID, responseEnvelope);
                        }
                        else if(requestEnvelope.RequestMethod == "DELETE")
                        {
                            responseEnvelope = Delete(requestEnvelope);
                            SendResponse(ClientProxyGUID, responseEnvelope);
                        }
                        else
                        {
                            //NOTE: Echo it back.
                            SendResponse(ClientProxyGUID, message);
                        }       
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

        public bool SendResponse(string ClientProxyGUID, string responseBody)
        {
            try
            {
                if (MessageBusBank == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusBankCannotBeNull);

               return MessageBusBank.ResolveMessageBus(ClientProxyGUID).SendMessage(responseBody);                
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
              
        public string Put(IChatMessageEnvelope request)
        {
            try
            {
                if (Tack == null)
                    throw new InvalidOperationException(ExceptionMessage_ITackCannotBeNull); 
                else
                {
                    IChatMessageEnvelope responseEnvelope = (IChatMessageEnvelope)Tack.PUT(request);
                    string responseString = _marshaller.MarshallPayloadJSON(responseEnvelope);
                    return responseString;
                }
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public string Post(IChatMessageEnvelope request)
        {
            try
            {
                if (Tack == null)
                    throw new InvalidOperationException(ExceptionMessage_ITackCannotBeNull);
                else
                {
                    IChatMessageEnvelope responseEnvelope = (IChatMessageEnvelope)Tack.POST(request);
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

        public string Delete(IChatMessageEnvelope request)
        {
            try
            {
                if (Tack == null)
                    throw new InvalidOperationException(ExceptionMessage_ITackCannotBeNull);
                else
                {
                    IChatMessageEnvelope responseEnvelope = (IChatMessageEnvelope)Tack.DELETE(request);
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
    }
}
