using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPersistence.Models.ChatMessage
{
    public class Channel
    {
        [Key]
        public long ChannelID { get; set; }
        public string ChannelName { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
