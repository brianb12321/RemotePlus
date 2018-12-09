using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    public class StringResource : Resource
    {
        [DataMember]
        public string Value { get; set; }
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