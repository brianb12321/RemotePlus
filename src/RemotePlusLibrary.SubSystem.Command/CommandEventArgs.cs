using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    public class CommandEventArgs : EventArgs
    {
        public CommandRequest Request { get; private set; }
        public CommandEventArgs(CommandRequest request)
        {
            Request = request;
        }
    }
}