using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.CommonUI.Requests
{
    public class SendLocalFileByteStreamRequest : StandordRequest<SendLocalFileByteStreamRequestBuilder>
    {
        IRemote _remote;
        public SendLocalFileByteStreamRequest(IRemote remote)
        {
            _remote = remote;
        }
        public override bool ShowProperties => false;

        public override string FriendlyName => "Send File Byte Stream Package Request";

        public override string Description => "Asks the client for a specific file byte stream package.";

        public override string URI => "global_sendByteStreamFilePackage";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(SendLocalFileByteStreamRequestBuilder builder, NetworkSide executingSide)
        {
            try
            {
                byte[] data = File.ReadAllBytes(builder.FileName);
                _remote.UploadBytesToResource(data, data.Length, builder.FileName);
                return RawDataResponse.Success(null);
            }
            catch (Exception ex)
            {
                GlobalServices.Logger.Log($"An error occurred in sending byte data to the server. Error: " + ex, BetterLogger.LogLevel.Error);
                return RawDataResponse.Cancel();
            }
        }

        public override void Update(string message)
        {
            throw new NotImplementedException();
        }
    }
}