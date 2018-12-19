using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class StringCommandElement : ICommandElement
    {
        [DataMember]
        public object Value { get; set; }

        public StringCommandElement(string content)
        {
            Value = content;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
        public bool IsOfType<TType>()
        {
            return Value is TType;
        }
    }
}
