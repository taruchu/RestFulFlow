using System.Collections.Generic;
using SharedServices.Interfaces.Envelope;
using SharedServices.Models.Constants;

namespace SharedServices.Models.EnvelopeModel
{
    public class EnvelopeModel : IEnvelope
    { 
       
        public Dictionary<string, string> Header_KeyValues { get; set; }
        public Dictionary<string, string> Filter_KeyValues { get; set; }
        public Dictionary<string, string> Payload_KeyValues { get; set; }
        public string JsonSchemas { get; private set; }

        public EnvelopeModel()
        { 
            Header_KeyValues = new Dictionary<string, string>();
            Filter_KeyValues = new Dictionary<string, string>();
            Payload_KeyValues = new Dictionary<string, string>();
        }

        public void InitializeThisEnvelopeFor_RoutingService()
        {
            this.Header_KeyValues.Clear();
            this.Header_KeyValues.Add(JSONSchemas.SenderRoute, string.Empty);
            this.Header_KeyValues.Add(JSONSchemas.DestinationRoute, string.Empty);
            this.Payload_KeyValues.Clear();
            this.Payload_KeyValues.Add(JSONSchemas.RoutingServiceCommand, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.Route, string.Empty);
        }

        public void InitializeThisEnvelopeFor_ServiceFarm()
        {
            this.Header_KeyValues.Clear();
            this.Header_KeyValues.Add(JSONSchemas.SenderRoute, string.Empty);
            this.Header_KeyValues.Add(JSONSchemas.DestinationRoute, string.Empty);
            this.Payload_KeyValues.Clear();
            this.Payload_KeyValues.Add(JSONSchemas.ListenersIpAddress, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ListenersPort, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.RequestedServiceName, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ServiceFarmCommand, string.Empty); 
        }

        public void InitializeThisEnvelopeFor_PersistenceService()
        {
            this.Header_KeyValues.Clear();
            this.Header_KeyValues.Add(JSONSchemas.SenderRoute, string.Empty);
            this.Header_KeyValues.Add(JSONSchemas.DestinationRoute, string.Empty);
            this.Payload_KeyValues.Clear();
            this.Payload_KeyValues.Add(JSONSchemas.PersistenceServiceCommand, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.StorageMechanism, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.StorageID, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.StoragePayload, string.Empty); 
        }

        public void InitializeThisEnvelopeFor_ChatMessageService()
        {
            this.Header_KeyValues.Clear();
            this.Header_KeyValues.Add(JSONSchemas.SenderRoute, string.Empty);
            this.Header_KeyValues.Add(JSONSchemas.DestinationRoute, string.Empty);
            this.Payload_KeyValues.Clear();
            this.Payload_KeyValues.Add(JSONSchemas.ChatMessageChannelName, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ChatMessageServiceCommand, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ChatMessageBody, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ChatMessageGUID, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ChatMessagePostedDateTimeStamp, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ChatMessageSender, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ChatMessage, string.Empty);
            this.Payload_KeyValues.Add(JSONSchemas.ChatMessageTags, string.Empty); 
        }
    }
}
