using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Client
{
    public class ContractTextConsoleStream : ITextStream
    {
        IRemoteClient _client = null;
        Guid _guid;
        public ContractTextConsoleStream(IRemoteClient client, Guid guid)
        {
            _client = client;
            _guid = guid;
        }
        public void Print(string message)
        {
            _client.TellMessageToServerConsoleNoNewLine(_guid, message);
        }

        public void Print(FormattedText text)
        {
            _client.TellMessageToServerConsoleNoNewLine(_guid, text);
        }

        public void PrintLine(string message)
        {
            _client.TellMessageToServerConsole(_guid, message);
        }

        public void PrintLine(FormattedText text)
        {
            _client.TellMessageToServerConsole(_guid, text);
        }

        public string Read()
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }
    }
}
