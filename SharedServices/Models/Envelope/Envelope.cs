using System;
using System.Collections.Generic;
using Newtonsoft.Json.Schema.Generation;
using SharedServices.Interfaces.Envelope;
using SharedServices.Models.Constants;

namespace SharedServices.Models.EnvelopeModel
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
