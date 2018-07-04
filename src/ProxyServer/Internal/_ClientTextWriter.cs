using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProxyServer.Internal
{
    public class _ClientTextWriter : TextWriter
    {
        public override void Write(char value)
        {
            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, value.ToString());
        }

        public override void Write(string value)
        {
            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
