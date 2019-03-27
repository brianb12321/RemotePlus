using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RemotePlusServer.Core;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.SubSystem.Command;

namespace RemotePlusServer.Internal
{
    public class _ClientTextWriter : TextWriter
    {
        public RemoteClient _client;
        public _ClientTextWriter(RemoteClient client)
        {
            _client = client;
        }
        public override void Write(char value)
        {
            _client.TellMessageToServerConsoleNoNewLine(value.ToString());
        }
        public override void WriteLine(char value)
        {
            _client.TellMessageToServerConsole(value.ToString());
        }

        public override void Write(string value)
        {
            _client.TellMessageToServerConsoleNoNewLine(value);
        }
        public override void WriteLine(string value)
        {
            _client.TellMessageToServerConsole(value);
        }
        public override void Write(object value)
        {
            _client.TellMessageToServerConsoleNoNewLine(value.ToString());
        }
        public override void WriteLine(object value)
        {
            if (value is ConsoleText)
            {
                _client.TellMessageToServerConsole((ConsoleText)value);
            }
            else
            {
                _client.TellMessageToServerConsole(value.ToString());
            }
        }
        public override void WriteLine()
        {
            _client.TellMessageToServerConsole("");
        }
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
