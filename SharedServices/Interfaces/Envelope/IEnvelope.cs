using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharedServices.Interfaces.Envelope
{
    public interface IEnvelope
    {
        /*
         * TODO: Remove key value pairs.
         * This interface will represent the base interface for all service envelopes.
         * It will contain a function for returning the JSON schema of the envelope.
         * It will contain a function for returning the envelope type.
         * It will contain properties that all services have in common like headers.
         * 
         */
         
        [Required]        
        string ServiceRoute { get; set; } 
        [Required]
        string ClientProxyGUID { get; set; } 
        [Required]
        string RequestMethod { get; set; }

        string GetMyJSONSchema();
        Type GetMyEnvelopeType(); 
    }
}