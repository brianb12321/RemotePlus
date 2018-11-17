using RemotePlusClient.CommonUI;
using RemotePlusClient.CommonUI.Controls.FileBrowserHelpers;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusClient.CommonUI.Connection;
using System.Windows.Forms;
using RemotePlusLibrary.Core;

namespace RemotePlusClient.CommonUI.Requests
{
    public class SelectFileRequest : StandordRequest<SelectFileRequestBuilder>
    {
        FindRemoteFileDialog fd = null;
        public override bool ShowProperties => false;

        public override string FriendlyName => "Select File Data Request";

        public override string Description => "Stuff";

        public override string URI => "global_selectFile";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(SelectFileRequestBuilder builder, NetworkSide executingSide)
        {
            Connection.Connection c = IOCContainer.GetService<Connection.Connection>();
            fd = new FindRemoteFileDialog(FilterMode.File, c.RemoteConnection, c.BaseAddress, c.Port);
            if (fd.ShowDialog() == DialogResult.OK)
            {
                return RawDataResponse.Success(fd.FilePath);
            }
            else
            {
                return RawDataResponse.Cancel();
            }
        }

        public override void Update(string message)
        {
            fd.Counter = int.Parse(message, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
