using log4net;
using log4net.Config;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SharedInterfaces.Interfaces.Routing;
using SharedServices.Models.Constants;
using System;
using System.Collections.Concurrent;



namespace SharedServices.Services.Routing
{
    public class MessageBus<T> : IMessageBus<T>
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MessageBus<T>));
        private object _thisLock { get; set; }
        public string MessageBusGUID
        {
            get
            {
                lock (_thisLock)
                {
                    if (String.IsNullOrEmpty(_messageBusGUID))
                        _messageBusGUID = Guid.NewGuid().ToString();
                    return _messageBusGUID; 
                }
            }
        }
        private string _messageBusGUID { get; set; }
        private ConcurrentQueue<T> _bus { get; set; }
        public Func<T, string> JsonSchema { get; set; }
        public string ExceptionMessage_JSONSchemaCannotBeNullOrEmpty
        {
            get
            {
                return "MessageBus<T> - JsonSchema is null or empty.";
            }
        }

        public string ExceptionMessage_MessageCannotBeNull
        {
            get
            {
                return "MessageBus<T> - Message cannot be null.";
            }
        }

        public MessageBus()
        {
            _thisLock = new object();
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(ConfigurationConstants.FileName_log4NetConfiguration));
            _bus = new ConcurrentQueue<T>(); 
        }
        public void Dispose()
        {  
            
        }

        public T ReceiveMessage()
        {
           lock(_thisLock)
            {
                try
                {
                    T message;
                    if (_bus.TryDequeue(out message))
                        return message;
                    else
                        return default(T);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }
        }

        public bool SendMessage(T message)
        {
            lock (_thisLock)
            {
                try
                {
                    if(message == null)
                        throw new InvalidOperationException(ExceptionMessage_MessageCannotBeNull);
                    if (JsonSchema == null)
                        throw new InvalidOperationException(ExceptionMessage_JSONSchemaCannotBeNullOrEmpty);
                    else if (ValidateMessage(message, JsonSchema(message))) 
                        _bus.Enqueue(message);  
                    return true;  
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }
        }

        public bool ValidateMessage(T message, string jsonSchema)
        { 
            try
            {
                if (String.IsNullOrEmpty(jsonSchema))
                    throw new InvalidOperationException(ExceptionMessage_JSONSchemaCannotBeNullOrEmpty);
                JSchema schema = JSchema.Parse(jsonSchema);
                string messageToString = (string)Convert.ChangeType(message, typeof(string));
                JObject parseMessage = JObject.Parse(messageToString);
                if(parseMessage.IsValid(schema))
                    return true;
                else
                {
                    _log.Warn(String.Format("ValidateMessage() - Could not validate this message: \n {0} \n\n using this schema: \n {1}", messageToString, jsonSchema));
                    return false;
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

        public bool IsEmpty()
        {
            lock(_thisLock)
            {
                return _bus.IsEmpty;
            }
        }
    }
}
