using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class PipelineCommandElement : ICommandElement
    {
        [DataMember]
        public object Value { get; set; }

        public bool IsOfType<TType>()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
