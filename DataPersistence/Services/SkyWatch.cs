using DataPersistence.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Services
{
    public class SkyWatch : ISkyWatch
    { 
        private ConcurrentDictionary<string, Dictionary<string, Action<ISkyWatchEventTypes, string>>> _watcherTable { get; set; }
        private object _thisLock { get; set; }
        private bool _isDisposed { get; set; }

        public SkyWatch()
        {
            _watcherTable = new ConcurrentDictionary<string, Dictionary<string, Action<ISkyWatchEventTypes, string>>>();
            _thisLock = new object();
        }

        public bool Declare(ISkyWatchEventTypes skyWatchEventType, string eventKey)
        {
            lock(_thisLock)
            {
                try
                {
                    string envelopeTypeString = eventKey.Split('.')[0]; 
                    Dictionary<string, Action<ISkyWatchEventTypes, string>> watchers;
                    if(_watcherTable.TryGetValue(envelopeTypeString, out watchers))
                    {
                        foreach(var eventHandler in watchers.Values)
                        {
                            eventHandler(skyWatchEventType, eventKey);
                        } 
                    } 
                    return true;
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public void Dispose()
        {
            lock (_thisLock)
            {
                try
                {
                    if (_isDisposed == false)
                    {
                        _watcherTable.Clear();
                        _isDisposed = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public bool UnWatch(string envelopeTypeString, string watcherGUID)
        {
            lock (_thisLock)
            {
                try
                { 
                    Dictionary<string, Action<ISkyWatchEventTypes, string>> watchers;
                    if(_watcherTable.TryGetValue(envelopeTypeString, out watchers))
                    {
                        watchers.Remove(watcherGUID);
                    }                    
                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public bool Watch(string envelopeTypeString, string watcherGUID, Action<ISkyWatchEventTypes, string> eventHandler)
        {
            lock (_thisLock)
            {
                try
                { 
                    Dictionary<string, Action<ISkyWatchEventTypes, string>> watchers;
                    if (_watcherTable.ContainsKey(envelopeTypeString) == false)
                        _watcherTable.TryAdd(envelopeTypeString, new Dictionary<string, Action<ISkyWatchEventTypes, string>>());
                    if(_watcherTable.TryGetValue(envelopeTypeString, out watchers))
                    {
                        watchers.Add(watcherGUID, eventHandler);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }
    }
}
