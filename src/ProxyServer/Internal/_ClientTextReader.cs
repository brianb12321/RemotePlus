using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer.Internal
{
    public class _ClientTextReader : TextReader
    {
        Guid _guid = Guid.Empty;
        public _ClientTextReader(Guid guid)
        {
            _guid = guid;
        }

        public override string ReadLine()
        {
            return ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.RequestInformation(_guid, new RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.ConsoleReadLineRequestBuilder("")).Data.ToString();
        }
    }
}