using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public enum ISkyWatchEventTypes { WriteOccured = 0 }

    public interface ISkyWatch : IDisposable
    {
        /*
         * This interface will allow clients to subscribe and get notified when an event happens.
         * Any group of clients that share the same SkyWatch instance can trigger/respond to the same events.
         * 
         */

        bool Declare(ISkyWatchEventTypes skyWatchEventType, string eventKey);//NOTE: Trigger an event.
        bool Watch(string eventKey, string watcherGUID, Action<ISkyWatchEventTypes, string> eventHandler);
        bool UnWatch(string eventKey, string watcherGUID);
    }
}
