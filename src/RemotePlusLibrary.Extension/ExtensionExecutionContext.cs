using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public class ExtensionExecutionContext
    {
        [DataMember]
        public CallType Mode { get; private set; }
        [DataMember]
        public Dictionary<string, string> ExtraData { get; set; }
        public ExtensionExecutionContext(CallType mode)
        {
            Mode = mode;
            ExtraData = new Dictionary<string, string>();
        }
    }
}