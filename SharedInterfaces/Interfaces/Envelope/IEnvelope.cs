using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace SharedInterfaces.Interfaces.Envelope
{
    public interface IEnvelope
    {
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