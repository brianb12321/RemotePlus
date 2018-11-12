using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.FileTransfer.Service;
using System;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;

namespace RemotePlusClientCmd.Requests
{
    public class SendFilePackageRequest : IDataRequest
    {
        public bool ShowProperties => false;

        public string FriendlyName => "Send File Package Request";

        public string Description => "Asks the client for a specific file package.";

        public string URI => "global_selectFilePackage";

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            try
            {
                new FileTransfer(ClientCmdManager.CurrentConnectionData.BaseAddress, ClientCmdManager.CurrentConnectionData.Port).SendFile(builder.UnsafeResolve<FileRequestOptions>().FileName);
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