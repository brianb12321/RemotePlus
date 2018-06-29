using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary;
using RemotePlusLibrary.RequestSystem;
using RemotePlusServer.Core;

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
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(Build()).Data.ToString();
        }
    }
}
