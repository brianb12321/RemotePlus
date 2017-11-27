using Logging;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public class ExtensionReturn
    {
        [DataMember]
        public int ReturnCode { get; set; }
        [DataMember]
        public object Data { get; set; }
        public ExtensionReturn(int rc)
        {
            ReturnCode = rc;
        }
    }
}