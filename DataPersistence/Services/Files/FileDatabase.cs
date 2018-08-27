using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DataPersistence.Services.Files
{
    [DataContract]
    public class FileDatabase
    {
        /// <summary>
        /// This blob stores strings using a string hash key. The strings could be JSON, or maybe a base64 string of bytes, etc.
        /// </summary>
        [DataMember]
        public Dictionary<string, string> BlobTable { get; set; }
        public FileDatabase()
        {
            BlobTable = new Dictionary<string, string>();
        }
    }
}
