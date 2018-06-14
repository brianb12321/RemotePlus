using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.AccountSystem
{
    [DataContract]
    public abstract class SecurityPolicy
    {
        [DataMember]
        public string PolicyEditorType { get; set; } = "Default";
        [DataMember(IsRequired = true)]
        public string ShortName { get; set; }
        [DataMember(IsRequired = true)]
        public string Path { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember(IsRequired = true)]
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }
}