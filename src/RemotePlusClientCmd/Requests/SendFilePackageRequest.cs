using RemotePlusClient.CommonUI;
using RemotePlusLibrary.RequestSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClientCmd.Requests
{
    public class SendFilePackageRequest : IDataRequest
    {
        public bool ShowProperties => false;

        public string FriendlyName => "Send File Package Request";

        public string Description => "Asks the client for a specific file package.";

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            try
            {
                new FileTransfer(ClientCmdManager.CurrentConnectionData.BaseAddress, ClientCmdManager.CurrentConnectionData.Port).SendFile(builder.Message);
                return RawDataRequest.Success(null);
            }
            catch
            {
                return RawDataRequest.Cancel();
            }
        }

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        public void UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
