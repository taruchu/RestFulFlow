using System;
using Newtonsoft.Json.Schema.Generation;
using SharedInterfaces.Interfaces.Envelope;

namespace SharedInterfaces.Models.EnvelopeModel
{
    public class EnvelopeModel : IEnvelope
    { 
        public EnvelopeModel()
        { 
             
        }

        public string ServiceRoute { get; set; }
        public string ClientProxyGUID { get; set; }
        public string RequestMethod { get; set; }

        public Type GetMyEnvelopeType()
        {
            return typeof(IEnvelope);
        }

        public string GetMyJSONSchema()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            return generator.Generate(typeof(IEnvelope)).ToString();
        }
    }
}
