using System.Collections.Generic;
using System.Runtime.Serialization;
using RemotePlusLibrary.Extension.ExtensionTypes.ExtensionDetailTypes;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public class ExtensionExecutionContext
    {
        [DataMember]
        public CallType Mode { get; private set; }
        [DataMember]
        public Dictionary<string, string> ExtraData { get; set; }
        [DataMember]
        public ClientExtensionDetails ClientExtension { get; set; } = null;
        public ExtensionExecutionContext(CallType mode)
        {
            Mode = mode;
            ExtraData = new Dictionary<string, string>();
        }
    }
}