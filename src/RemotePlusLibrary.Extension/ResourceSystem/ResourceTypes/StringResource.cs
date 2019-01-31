using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    [Serializable]
    public class StringResource : Resource
    {
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public override string ResourceType => "String";
        [DataMember]

        public override bool SaveToFile { get; set; } = true;

        public override string ToString()
        {
            return Value;
        }
        public StringResource(string id, string value) : base(id)
        {
            Value = value;
        }
    }
}