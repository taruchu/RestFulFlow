using log4net;
using log4net.Config;
using SharedServices.Interfaces.Routing;
using SharedServices.Models.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Services.Routing
{
    public class MessageBusBank<T> : IMessageBusBank<T>
    {
        public string MessageBusBankGUID
        {
            get
            {
                lock (_thisLock)
                {
                    if (String.IsNullOrEmpty(_messageBusBankGUID))
                        _messageBusBankGUID = Guid.NewGuid().ToString();
                    return _messageBusBankGUID;
                }
            }
        }
        private string _messageBusBankGUID { get; set; }
        private ConcurrentDictionary<string, IMessageBus<T>> _bank;
        private static readonly ILog _log = LogManager.GetLogger(typeof(MessageBusBank<T>));
        private object _thisLock { get; set; }
        private bool _isDisposed { get; set; }

        public MessageBusBank()
        {
            _isDisposed = false;
            _thisLock = new object();
            _bank = new ConcurrentDictionary<string, IMessageBus<T>>();
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(ConfigurationConstants.FileName_log4NetConfiguration));
        }
        public string ExceptionMessage_BusKeyCodeCannotBeNullOrEmpty
        {
            get
            {
                return "MessageBusBank<T> - BusKeyCode cannot be null or empty.";
            } 
        }

        public string ExceptionMessage_MessgeBusCannotBeNull
        {
            get
            {
                return "MessageBusBank<T> - MessageBus cannot be null.";
            }
        }

        public void Dispose()
        {
            lock (_thisLock)
            {
                if (_isDisposed == false)
                {
                    foreach (IMessageBus<T> bus in _bank.Values)
                        bus.Dispose();

                    _bank.Clear();
                    _isDisposed = true; 
                }
            }
        }

        public bool RegisterMessageBus(string busKeyCode, IMessageBus<T> messageBus)
        { 
            lock(_thisLock)
            {
                try
                {
                    if (String.IsNullOrEmpty(busKeyCode))
                        throw new InvalidOperationException(ExceptionMessage_BusKeyCodeCannotBeNullOrEmpty);
                    else if (messageBus == null)
                        throw new InvalidOperationException(ExceptionMessage_MessgeBusCannotBeNull);
                    else
                    { 
                        return _bank.TryAdd(busKeyCode, messageBus);
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

        public bool ReleaseMessageBus(string busKeyCode)
        { 
            lock(_thisLock)
            {
                try
                {
                    if (String.IsNullOrEmpty(busKeyCode))
                        throw new InvalidOperationException(ExceptionMessage_BusKeyCodeCannotBeNullOrEmpty);
                    else
                    {
                        IMessageBus<T> removedMessageBus;
                        return _bank.TryRemove(busKeyCode, out removedMessageBus);
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

        public IMessageBus<T> ResolveMessageBus(string busKeyCode)
        { 
            lock (_thisLock)
            {
                try
                {
                    if (String.IsNullOrEmpty(busKeyCode))
                        throw new InvalidOperationException(ExceptionMessage_BusKeyCodeCannotBeNullOrEmpty);
                    else
                    {
                        IMessageBus<T> resolvedMessageBus = null;
                        _bank.TryGetValue(busKeyCode, out resolvedMessageBus);
                        return resolvedMessageBus;
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
        }

        public List<string> GetBusKeyCodes()
        {
            lock(_thisLock)
            {
                try
                {
                    return _bank.Keys.ToList<string>();
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }
    }
}
