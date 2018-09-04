using SharedUtilities.Interfaces.Marshall;
using SharedInterfaces.Interfaces.Proxy;
using SharedInterfaces.Interfaces.Routing;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace SharedServices.Services.Proxy
{
    public class ClientProxy : IClientProxy
    {
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
        public IMessageBusWriter<string> MessageBusWiter { get; set; }
        public IMessageBusReaderBank<string> MessageBusReaderBank { get; set; }
        public IMessageBusBank<string> MessageBusBank { get; set; }
        public IMarshaller Marshaller { get; set; }
        public Action<string> HandleMessageFromRouter { get; set; }
        private object _thisLock { get; set; }

        public string ExceptionMessage_MessageBusWriterCannotBeNull
        {
            get
            {
                return "ClientProxy - MessageBusWritter cannot be null.";
            }
        }

        public string ExceptionMessage_MessageBusReaderBankCannotBeNull
        {
            get
            {
                return "ClientProxy - MessageBusReaderBank cannot be null.";
            }
        }

        public string ExceptionMessage_MessageBusBankCannotBeNull
        {
            get
            {
                return "ClientProxy - MessageBusBank cannot be null.";
            }
        }

        public string ExceptionMessage_MarshallerCannotBeNull
        {
            get
            {
                return "ClientProxy - Marshaller cannot be null.";
            }
        }
        
        private IMarshaller _marshaller { get; set; }
        private bool _isDisposed { get; set; }

        public ClientProxy(IMarshaller marshaller)
        {
            _marshaller = marshaller;
            _thisLock = new object();
            HandleMessageFromRouter = EnqueueMessage;
        }

        public void Dispose()
        {
            if(_isDisposed == false)
            {
                MessageBusReaderBank.Dispose();
                MessageBusWiter.Dispose();
            }
        }

        public void EnqueueMessage(string message)
        {
            lock (_thisLock)
            {
                try
                {
                    if (MessageBusWiter == null)
                        throw new InvalidOperationException(ExceptionMessage_MessageBusWriterCannotBeNull);
                    MessageBusWiter.Write(message);
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
        
        public string PollMessageBus(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                if (MessageBusReaderBank == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusReaderBankCannotBeNull); 
               
                Task<string> messageTask = MessageBusReaderBank.PollMessageBusForSingleMessage(cancellationTokenSource);
                messageTask.Wait();
                switch (messageTask.Status)
                { 
                    case TaskStatus.RanToCompletion:
                        return messageTask.Result;
                    case TaskStatus.Faulted:
                        throw new ApplicationException(messageTask.Exception.Flatten().InnerException.Message, messageTask.Exception.Flatten().InnerException);
                    default:
                        return String.Empty;
                }                
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public bool SendResponse(string clientProxyGUID, string responseBody)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
