using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Client
{
    public class ContractTextStream : ITextStream
    {
        IRemoteClient _client = null;
        Guid _guid;
        public ContractTextStream(IRemoteClient client, Guid guid)
        {
            _client = client;
            _guid = guid;
        }
        public void Print(string message)
        {
            _client.TellMessage(_guid, message);
        }

        public void Print(FormattedText text)
        {
            _client.TellMessage(_guid, text);
        }

        public void PrintLine(string message)
        {
            _client.TellMessage(_guid, message);
        }

        public void PrintLine(FormattedText text)
        {
            _client.TellMessage(_guid, text);
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
