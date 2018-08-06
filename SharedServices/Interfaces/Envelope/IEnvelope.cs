using System.Collections.Generic;

namespace SharedServices.Interfaces.Envelope
{
    public interface IEnvelope
    {
        //Refactor NOTE: This interface will represent a un-Marshalled JSON message passed around using message buses.
        //Need to refactor this so that it contains key,value pair tables. One for
        //the filter and the other for the payload. Also there should be a Header key, value 
        //pair table for defining such things as sender route and destination route. This is important so 
        //that after a service processes a request, they can send the response back to the sender route using
        //the routing service's message bus. Each service will have a reference to the message bus bank and they will also
        //have the <RouterBusKeyCode> for their local routing service. This will allow them to send messages to through their router.
        //all other coupling between services will be removed.

        //Each service's message bus
        //will define a JSON schema to validate this envelope before it consumes it. If it doesn't pass validation
        //the message bus should log the error for debugging and then drop the envelope.
        //
         
        Dictionary<string, string> Header_KeyValues { get; set; }
        Dictionary<string, string> Filter_KeyValues { get; set; }
        Dictionary<string, string> Payload_KeyValues { get; set; }
        void InitializeThisEnvelopeFor_RoutingService();
        void InitializeThisEnvelopeFor_PersistenceService();
        void InitializeThisEnvelopeFor_ChatMessageService();
    }
}