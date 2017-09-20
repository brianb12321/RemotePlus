using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.Programmer
{
    public class ServerExtensionProgrammerUpdateEvent
    {
        public ServerExtensionProgrammer Programmer { get; private set; }
        public bool Cancel { get; set; } = true;
        public ServerExtensionProgrammerUpdateEvent(ServerExtensionProgrammer programmer)
        {
            Programmer = programmer;
        }
    }
}
