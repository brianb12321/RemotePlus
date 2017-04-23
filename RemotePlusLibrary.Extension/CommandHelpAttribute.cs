using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandHelpAttribute : Attribute
    {
        public string HelpMessage { get; private set; }
        public CommandHelpAttribute(string Message)
        {
            HelpMessage = Message;
        }
    }
}
