using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class ScriptCommandElement : ICommandElement
    {
        public ScriptCommandElement(object value)
        {
            Value = value ?? string.Empty;
            StringValue = Value.ToString();
        }
        [IgnoreDataMember]
        public object Value { get; private set; }
        [DataMember]
        public string StringValue { get; private set; }

        public bool IsOfType<TType>()
        {
            return Value is TType;
        }
        public override string ToString()
        {
            //This class is special
            if(Value is ResourceQuery)
            {
                return IOCContainer.GetService<IResourceManager>().GetResource<Resource>((ResourceQuery)Value).ToString();
            }
            return Value.ToString();
        }
    }
}