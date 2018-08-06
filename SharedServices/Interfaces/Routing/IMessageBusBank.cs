using System;
using System.Collections.Generic;

namespace SharedServices.Interfaces.Routing
{
    //NOTE: This is a thread safe global bank of various message buses. 
    public interface IMessageBusBank<T> : IDisposable
    {
        string MessageBusBankGUID { get;}
        bool RegisterMessageBus(string busKeyCode, IMessageBus<T> messageBus);        
        IMessageBus<T> ResolveMessageBus(string busKeyCode); //NOTE: If cannot resolve log error for debugging and then drop the request.
        bool ReleaseMessageBus(string busKeyCode);
        List<string> GetBusKeyCodes();
        string ExceptionMessage_BusKeyCodeCannotBeNullOrEmpty { get; }
        string ExceptionMessage_MessgeBusCannotBeNull { get; }
    }
}
