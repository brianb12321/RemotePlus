using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class ResourceQueryCommandElement : ICommandElement
    {
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public ResourceQuery Query { get => (ResourceQuery)Value; set => Value = value; }
        public ElementValueType ValueType => ElementValueType.Value;

        public ResourceQueryCommandElement(ResourceQuery rq)
        {
            Value = rq;
        }
        public override string ToString()
        {
            return IOCContainer.GetService<IResourceManager>().GetResource<Resource>(Query).ToString();
        }
        public bool IsOfType<TType>()
        {
            return Value is TType;
        }
    }
}
