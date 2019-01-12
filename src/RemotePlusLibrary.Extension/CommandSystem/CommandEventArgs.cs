using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
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