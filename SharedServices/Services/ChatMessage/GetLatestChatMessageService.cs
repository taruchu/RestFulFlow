using ChatMessageInterfaces.Interfaces.ChatMessage;
using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Routing;
using SharedUtilities.Interfaces.Marshall;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedServices.Services.ChatMessage
{
    public class GetLatestChatMessageService : IGetLatestChatMessageService
    {
        public ITack Tack { get; set; } 
        public string ServiceName { get; set; }
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
        private IMarshaller _marshaller { get; set; }
        private ConcurrentQueue<IChatMessageEnvelope> _skyWatchQueue { get; set; }
        private IChatMessageEnvelopeFactory _chatMessageEnvelopeFactory { get; set; }

        public IMessageBusWriter<string> MessageBusWiter { get; set; }
        public IMessageBusReaderBank<string> MessageBusReaderBank { get; set; }
        public IMessageBusBank<string> MessageBusBank { get; set; }
        public Action<string> HandleMessageFromRouter { get; set; }

        public string ExceptionMessage_ITackCannotBeNull
        {
            get
            {
                return "GetLatestChatMessage - ITack cannot be null.";
            }
        }
        public string ExceptionMessage_MessageBusWriterCannotBeNull
        {
            get
            {
                return "GetLatestChatMessage - MessageBusWriter cannot be null.";
            }
        }
        public string ExceptionMessage_MessageBusReaderBankCannotBeNull
        {
            get
            {
                return "GetLatestChatMessage - MessageBusReaderBank cannot be null.";
            }
        }
        public string ExceptionMessage_MessageBusBankCannotBeNull
        {
            get
            {
                return "GetLatestChatMessage - MessageBusBank cannot be null.";
            }
        }
        public string ExceptionMessage_MarshallerCannotBeNull
        {
            get
            {
                return "GetLatestChatMessage - Marshaller cannot be null.";
            }
        }

        public double PollDurrationInMinutes
        {
            get
            {
                return (_pollDurration == 0) ? 1.0 : _pollDurration;
            }
            set
            {
                _pollDurration = value;
            }
        }

        private double _pollDurration { get; set; }

        public GetLatestChatMessageService(IMarshaller marshaller, IChatMessageEnvelopeFactory chatMessageEnvelopeFactory)
        {
            _isDisposed = false;
            HandleMessageFromRouter = AddMessageToBus;
            _marshaller = marshaller;
            _thisLock = new object();
            _skyWatchQueue = new ConcurrentQueue<IChatMessageEnvelope>();
            chatMessageEnvelopeFactory = _chatMessageEnvelopeFactory;
            _pollDurration = 0;
        }

        public void Dispose()
        {
            try
            {
                if (_isDisposed == false)
                {
                    MessageBusWiter.Dispose();
                    MessageBusReaderBank.Dispose();
                    Tack.SkyWatch.UnWatch(typeof(IChatMessageEnvelope).ToString(), ServiceGUID); //NOTE: Must stop watching since SkyWatch is a global instance.
                    Tack.Dispose();
                    _isDisposed = true;
                }
            }
            catch (Exception ex)
            {
                new ApplicationException(ex.Message, ex);
            }
        }

        public void AddMessageToBus(string message)
        {
            lock (_thisLock)
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
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        private void SkyWatchEventHandler(ISkyWatchEventTypes skyWatchEventType, string eventKey)
        {
            lock (_thisLock)
            {
                try
                {
                    if (skyWatchEventType == ISkyWatchEventTypes.WriteOccured)
                    {
                        Type envelopeType = Tack.GetIEnvelopeType(eventKey);
                        long ID = Tack.GetStorageID(eventKey);
                        IChatMessageEnvelope eventSubject_ChatMessageEnvelope = _chatMessageEnvelopeFactory.InstantiateIEnvelope();
                        eventSubject_ChatMessageEnvelope.ChatMessageID = ID;
                        IChatMessageEnvelope chatMessageEnvelopeFromSkyWatch = GetByID(eventSubject_ChatMessageEnvelope);
                        _skyWatchQueue.Enqueue(chatMessageEnvelopeFromSkyWatch);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
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
                            IChatMessageEnvelope nextChatMessage = chatMessageQueryRepo.GetLatestChatMessage(chatMessageEnvelopeParam);
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

        public IChatMessageEnvelope GetByID(IChatMessageEnvelope request)
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
                            IChatMessageEnvelope nextChatMessage = chatMessageQueryRepo.GetChatMessageByID(chatMessageEnvelopeParam);
                            return nextChatMessage;
                        };

                    return (IChatMessageEnvelope)Tack.GET(request); 
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
                        string ClientProxyGUID = requestEnvelope.ClientProxyGUID; 

                        //NOTE: Get whatever is latest in the data source and send it to the client.
                        string responseEnvelope = Get(requestEnvelope); 
                        SendResponse(ClientProxyGUID, responseEnvelope);

                        //NOTE: Now subscribe to skywatch and wait for anything new.
                        //When it comes in send it to the client proxy and continue.
                        string responseFromSkyWatch = string.Empty; 
                        Tack.SkyWatch.Watch(typeof(IChatMessageEnvelope).ToString(), ServiceGUID, SkyWatchEventHandler);

                        //NOTE: Break after poll durration to avoid infinite loop. OS multi-tasking will preempt, but since this service 
                        //can't be invoked directly by the client proxy, I need a way to stop this particular threads loop without envolving the client,
                        //and without stopping the loops in the other threads by unwatching (they share the same GUID). 
                        DateTime endTime = DateTime.Now.AddMinutes(PollDurrationInMinutes);
                        while(_skyWatchQueue.IsEmpty) { }
                        while (_skyWatchQueue.IsEmpty == false && DateTime.Compare(DateTime.Now, endTime) < 0)
                        {
                            IChatMessageEnvelope chatMessageEnvelopeFromSkyWatch;
                            if (_skyWatchQueue.TryDequeue(out chatMessageEnvelopeFromSkyWatch))
                            {
                                responseFromSkyWatch = _marshaller.MarshallPayloadJSON(chatMessageEnvelopeFromSkyWatch);
                                SendResponse(ClientProxyGUID, responseFromSkyWatch);
                            }    
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
