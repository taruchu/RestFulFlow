using System;

namespace SharedServices.Interfaces.Routing
{
    public interface IMessageBus<T> : IDisposable
    {
        //TODO: The message bus will gate privilages from all incomming messages. And it
        // will validate all messages using a JSON schema. 

        Func<T, string> JsonSchema { get; set; }
        string MessageBusGUID { get; } //NOTE: Must be thread safe. Will create the GUID on the first get.
        bool SendMessage(T message); //NOTE: Must be thread safe.
        T ReceiveMessage(); //NOTE: Must be thread safe.
        bool ValidateMessage(T message, string jsonSchema);
        bool IsEmpty();
        string ExceptionMessage_JSONSchemaCannotBeNullOrEmpty { get; }
        string ExceptionMessage_MessageCannotBeNull { get; }
    }
}
