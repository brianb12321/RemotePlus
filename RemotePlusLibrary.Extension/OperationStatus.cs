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
        public object Data { get; set; }
        public OperationStatus()
        {

        }
    }
}