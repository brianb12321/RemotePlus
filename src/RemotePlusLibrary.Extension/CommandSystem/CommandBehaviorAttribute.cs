using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [DataContract]
    public class CommandBehaviorAttribute : Attribute
    {
        [DataMember]
        public CommandExecutionMode ExecutionType { get; set; } = CommandExecutionMode.Client;
        [DataMember]
        public bool IndexCommandInHelp { get; set; } = true;
        [DataMember]
        public bool DoNotCatchExceptions { get; set; }
        [DataMember]
        public StatusCodeDeliveryMethod StatusCodeDeliveryMethod { get; set; } = StatusCodeDeliveryMethod.DoNotDeliver;
    }
}