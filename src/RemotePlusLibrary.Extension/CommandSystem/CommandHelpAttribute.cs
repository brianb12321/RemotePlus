using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    [AttributeUsage(AttributeTargets.Method)]
    [DataContract]
    public class CommandHelpAttribute : Attribute
    {
        [DataMember]
        public string HelpMessage { get; private set; }
        public CommandHelpAttribute(string Message)
        {
            HelpMessage = Message;
        }
    }
}
