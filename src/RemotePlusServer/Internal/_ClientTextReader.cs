using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RemotePlusServer.Core;

namespace RemotePlusServer.Internal
{
    class _ClientTextReader : TextReader
    {
        public override string ReadLine()
        {
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(new RemotePlusLibrary.RequestSystem.RequestBuilder("rcmd_textBox", "", null)).Data.ToString();
        }
    }
}
