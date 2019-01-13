using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RemotePlusLibrary.Extension.CommandSystem;

namespace ProxyServer.Internal
{
    public class _ClientTextWriter : TextWriter
    {
        Guid _guid = Guid.Empty;
        public _ClientTextWriter(Guid guid)
        {
            _guid = guid;
        }
        public override void Write(char value)
        {
            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsoleNoNewLine(_guid, value.ToString());
        }
        public override void WriteLine(char value)
        {
            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(_guid, value.ToString());
        }

        public override void Write(string value)
        {
            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsoleNoNewLine(_guid, value);
        }
        public override void WriteLine(string value)
        {
            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(_guid, value);
        }
        public override void Write(object value)
        {
            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsoleNoNewLine(_guid, value.ToString());
        }
        public override void WriteLine(object value)
        {
            if (value is ConsoleText)
            {
                ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(_guid, (ConsoleText)value);
            }
            else
            {
                ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(_guid, value.ToString());
            }
        }
        public override void WriteLine()
        {
            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(_guid, "");
        }
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
