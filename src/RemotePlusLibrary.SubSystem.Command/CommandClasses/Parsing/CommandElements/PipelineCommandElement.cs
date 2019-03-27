using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class PipelineCommandElement : ICommandElement
    {
        [IgnoreDataMember]
        private ICommandEnvironment _env;
        [IgnoreDataMember]
        private string _command;
        public PipelineCommandElement(string command, ICommandEnvironment env)
        {
            _command = command;
            _env = env;
        }
        [DataMember]
        public object Value { get; set; }
        [IgnoreDataMember]
        public CommandPipeline Pipeline => (CommandPipeline)Value;

        public ElementValueType ValueType => ElementValueType.Value;

        public void Execute()
        {
            Value = _env.Execute(_command, CommandExecutionMode.Client);
        }
        public bool IsOfType<TType>()
        {
            return Value is TType;
        }
        public override string ToString()
        {
            Execute();
            return Value.ToString();
        }
    }
}
