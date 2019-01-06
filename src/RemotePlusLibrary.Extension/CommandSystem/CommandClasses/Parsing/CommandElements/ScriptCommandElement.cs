using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Scripting;
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
        private string _script = string.Empty;
        public ScriptCommandElement(string script)
        {
            _script = script;
        }
        [IgnoreDataMember]
        public object Value { get; private set; }
        [DataMember]
        public string StringValue { get; private set; }

        public bool IsOfType<TType>()
        {
            return Value is TType;
        }
        public void Execute()
        {
            Value = IOCContainer.GetService<IScriptingEngine>().ExecuteStringUsingSameScriptScope(_script);
            if(Value != null)
            {
                StringValue = Value.ToString();
            }
            else
            {
                StringValue = string.Empty;
            }
        }
        public override string ToString()
        {
            Execute();
            return Value.ToString();
        }
    }
}