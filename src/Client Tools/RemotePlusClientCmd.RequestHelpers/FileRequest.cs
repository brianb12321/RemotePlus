using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary;

namespace RemotePlusClientCmd.RequestHelpers
{
    public class FileRequest : IURIWrapper<string>
    {
        public string URI { get; } = "global_selectFile";
        public FileRequest()
        {
        }
        public RequestBuilder Build()
        {
            return RequestBuilder.RequestFile();
        }

        public string BuildAndSend()
        {
            return RemotePlusServer.ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(Build()).Data.ToString();
        }
    }
}
