using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class AggregateCommandElement : ICommandElement
    {
        [DataMember]
        public object Value { get; } = new List<ICommandElement>();
        [DataMember]
        public List<ICommandElement> CommandElements => (List<ICommandElement>)Value;
        public AggregateCommandElement(List<ICommandElement> elements)
        {
            Value = elements;
        }
        public bool IsOfType<TType>()
        {
            return true;
        }
    }
}
