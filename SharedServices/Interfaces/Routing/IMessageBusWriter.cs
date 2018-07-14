using System;

namespace SharedServices.Interfaces.Routing
{
    public interface IMessageBusWriter<T> : IDisposable
    {
        string SpecifyTheMessageBus(IMessageBus<T> toWrite); //NOTE: Return the message bus GUID.
        bool Write(T message); //NOTE : Returns true if successfull. Must be thread safe.
        string ExceptionMessage_MessageCannotBeNullOrEmpty { get; }
        string ExceptionMessage_MessageBusCannotBeNull { get; }
    }
}
