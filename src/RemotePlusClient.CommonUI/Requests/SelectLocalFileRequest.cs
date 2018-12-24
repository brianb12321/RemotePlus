using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Requests
{
    public class SelectLocalFileRequest : StandordRequest<FileDialogRequestBuilder, UpdateRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Select Local File Request";

        public override string Description => "Requests for a local path. A local path is a path on the client.";

        public override string URI => "r_selectLocalFile";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(FileDialogRequestBuilder builder, NetworkSide executingSide)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = builder.Title;
            ofd.Filter = builder.Filter;
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                return RawDataResponse.Success(ofd.FileName);
            }
            else
            {
                return RawDataResponse.Cancel();
            }
        }
    }
}