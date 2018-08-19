using SharedInterfaces.Interfaces.Routing;
using System;

namespace SharedServices.Services.Routing
{
    public class MessageBusWriter<T> : IMessageBusWriter<T>
    {
        public string ExceptionMessage_MessageCannotBeNullOrEmpty
        {
            get
            {
                return "MessageBusWriter<T> - Message cannot be null or empty.";
            }
        } 
        public string ExceptionMessage_MessageBusCannotBeNull
        {
            get
            {
                return "MessageBusWriter<T> - MessageBus cannot be null.";
            }
        }
        private IMessageBus<T> _messageBus { get; set; }
        private bool _isDisposed { get; set; }

        public MessageBusWriter()
        {
            _isDisposed = false;
        }

        public void Dispose()
        {
            if(_isDisposed == false)
            {
                _messageBus = null;
                _isDisposed = true;
            }
        }

        public string SpecifyTheMessageBus(IMessageBus<T> toWrite)
        {
            try
            {
                if (toWrite == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusCannotBeNull);
                else
                {
                    _messageBus = toWrite;
                    return _messageBus.MessageBusGUID;
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

        public bool Write(T message)
        {
            try
            {
                if (typeof(T) == typeof(string) && String.IsNullOrEmpty((string)Convert.ChangeType(message, typeof(string))))
                    throw new InvalidOperationException(ExceptionMessage_MessageCannotBeNullOrEmpty);
                else if (message == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageCannotBeNullOrEmpty);
                else if (_messageBus == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusCannotBeNull);
                else
                {
                    return _messageBus.SendMessage(message);
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
    }
}
