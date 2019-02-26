using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RemotePlusServer.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Contracts;

namespace RemotePlusServer.Internal
{
    public class _ClientTextReader : TextReader
    {
        public RemoteClient _client;
        public _ClientTextReader(RemoteClient client)
        {
            _client = client;
        }
        public override string ReadLine()
        {
            return _client.RequestInformation(new RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.ConsoleReadLineRequestBuilder(string.Empty)).Data.ToString();
        }
    }
}