using Newtonsoft.Json.Schema.Generation;
using SharedInterfaces.Interfaces.Envelope;
using System;

namespace SharedServices.Models.Envelope
{
    public class ChatMessageEnvelope : IChatMessageEnvelope
    {
        //NOTE: Header
        public string ServiceRoute { get; set; }
        public string ClientProxyGUID { get; set; }
        public string RequestMethod { get; set; }

        //NOTE: Specific
        public long ChatMessageID { get; set; }
        public long ChatChannelID { get; set; }
        public string ChatChannelName { get; set; }
        public string SenderUserName { get; set; }
        public string ChatMessageBody { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        

        public Type GetMyEnvelopeType()
        {
            return typeof(IChatMessageEnvelope);
        }

        public string GetMyJSONSchema()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            return generator.Generate(typeof(IChatMessageEnvelope)).ToString();
        }
    }
}
