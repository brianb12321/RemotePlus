using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class ScriptCommandElement : ICommandElement
    {
        private string _script = string.Empty;
        ICommandEnvironment _environment;
        public ScriptCommandElement(string script, ICommandEnvironment environment)
        {
            _script = script;
            _environment = environment;
        }
        [IgnoreDataMember]
        public object Value { get; private set; }
        [DataMember]
        public string StringValue { get; private set; }
        public ElementValueType ValueType => ElementValueType.Value;

        public bool IsOfType<TType>()
        {
            return Value is TType;
        }
        public void Execute()
        {
            Value = _environment.ExecuteScript(_script);
            if (Value != null)
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