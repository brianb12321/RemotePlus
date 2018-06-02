using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RemotePlusServer.Internal
{
    public class _ClientTextWriter : TextWriter
    {
        public override void Write(char value)
        {
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(value.ToString());
        }

        public override void Write(string value)
        {
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
