using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.FileTransfer.Service;
using System;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Core;

namespace RemotePlusClient.CommonUI.Requests
{
    public class SendFilePackageRequest : StandordRequest<SendFilePackageRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Send File Package Request";

        public override string Description => "Asks the client for a specific file package.";

        public override string URI => "global_selectFilePackage";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(SendFilePackageRequestBuilder builder, NetworkSide executingSide)
        {
            try
            {
                Connection.Connection c = IOCContainer.GetService<Connection.Connection>();
                new FileTransfer(c.BaseAddress, c.Port).SendFile(builder.FileName);
                return RawDataResponse.Success(null);
            }
            catch
            {
                return RawDataResponse.Cancel();
            }
        }

        public override void Update(string message)
        {
            throw new NotImplementedException();
        }
    }
}