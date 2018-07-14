using System;

namespace SharedServices.Interfaces.Routing
{
    public interface IMessageBusReaderBank<T> : IDisposable
    {
        string SpecifyTheMessageBus(IMessageBus<T> toRead); //NOTE: Return the message bus GUID.
        int AddAnotherReader(Action<T> performedOnEachMessageRead); //NOTE: Returns the current reader count.  
        int DecreaseReaderBank(int byAmount);  //NOTE: Returns the current reader count.
        bool StopReading(); //NOTE: Returns true for success. 
        string ExceptionMessage_MessageBusCannotBeNull { get; }
        string ExceptionMessage_ActionPerformedOnEachMessageReadCannotBeNull { get; } 
    }
}
