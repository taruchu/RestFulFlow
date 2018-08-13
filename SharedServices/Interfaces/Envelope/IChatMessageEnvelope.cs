using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Interfaces.Envelope
{
    public interface IChatMessageEnvelope : IEnvelope
    {
        [Required]
        [DefaultValue(0)]
        long ChatMessageID { get; set; }

        [Required]
        [DefaultValue(0)]
        long ChatChannelID { get; set; }

        [Required]
        string ChatChannelName { get; set; }

        [Required]
        string SenderUserName { get; set; }

        [Required]
        string ChatMessageBody { get; set; } 
        
        [DataType(DataType.DateTime)]
        DateTime CreatedDateTime { get; set; }
    }
}
