using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandBehaviorAttribute : Attribute
    {
        public CommandExecutionMode ExecutionType { get; set; } = CommandExecutionMode.Client;
        public bool IndexCommandInHelp { get; set; } = true;
        public bool DoNotCatchExceptions { get; set; }
        public StatusCodeDeliveryMethod StatusCodeDeliveryMethod { get; set; } = StatusCodeDeliveryMethod.DoNotDeliver;
    }
}