using Logging;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public class OperationStatus
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public List<LogItem> Log { get; set; }
        public OperationStatus()
        {
            Log = new List<LogItem>();
        }
    }
}