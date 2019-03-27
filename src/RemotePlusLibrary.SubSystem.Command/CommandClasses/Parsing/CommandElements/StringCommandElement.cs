using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class StringCommandElement : ICommandElement
    {
        [DataMember]
        public object Value { get; set; }
        public ElementValueType ValueType { get; }

        public StringCommandElement(string content, ElementValueType type)
        {
            ValueType = type;
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
