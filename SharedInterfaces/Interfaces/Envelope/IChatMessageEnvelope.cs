using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces.Interfaces.Envelope
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

        [DataType(DataType.DateTime)]
        DateTime ModifiedDateTime { get; set; }

        Func<IChatMessageQueryRepository, IChatMessageEnvelope, IChatMessageEnvelope> Query { get; set; }
        Func<IChatMessageQueryRepository, IChatMessageEnvelope, List<IChatMessageEnvelope>> QueryForList { get; set; }
    }
}
