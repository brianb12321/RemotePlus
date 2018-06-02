using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RemotePlusServer.Internal
{
    class _ClientTextReader : TextReader
    {
        public override string ReadLine()
        {
            return ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(new RemotePlusLibrary.RequestBuilder("rcmd_textBox", "", null)).Data.ToString();
        }
    }
}
