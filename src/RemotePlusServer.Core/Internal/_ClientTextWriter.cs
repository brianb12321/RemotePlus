using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RemotePlusServer.Core;
using RemotePlusLibrary.Extension.CommandSystem;

namespace RemotePlusServer.Internal
{
    public class _ClientTextWriter : TextWriter
    {
        public override void Write(char value)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsoleNoNewLine(value.ToString());
        }
        public override void WriteLine(char value)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(value.ToString());
        }

        public override void Write(string value)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsoleNoNewLine(value);
        }
        public override void WriteLine(string value)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(value);
        }
        public override void Write(object value)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsoleNoNewLine(value.ToString());
        }
        public override void WriteLine(object value)
        {
            if (value is ConsoleText)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole((ConsoleText)value);
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(value.ToString());
            }
        }
        public override void WriteLine()
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("");
        }
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
