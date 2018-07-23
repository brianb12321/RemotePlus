using RemotePlusClient.CommonUI;
using RemotePlusClient.CommonUI.Controls.FileBrowserHelpers;
using RemotePlusLibrary;
using RemotePlusLibrary.RequestSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClientCmd.Requests
{
    public class SelectFileRequest : IDataRequest
    {
        FindRemoteFileDialog fd = null;
        public bool ShowProperties => false;

        public string FriendlyName => "Select File Data Request";

        public string Description => "Stuff";

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            if (ClientCmdManager.ProxyEnabled)
            {
                fd = new FindRemoteFileDialog(FilterMode.File, ClientCmdManager.Proxy, ClientCmdManager.CurrentConnectionData.BaseAddress, ClientCmdManager.CurrentConnectionData.Port);
            }
            else
            {
                fd = new FindRemoteFileDialog(FilterMode.File, ClientCmdManager.Remote, ClientCmdManager.CurrentConnectionData.BaseAddress, ClientCmdManager.CurrentConnectionData.Port);
            }
            if (fd.ShowDialog() == DialogResult.OK)
            {
                return RawDataRequest.Success(fd.FilePath);
            }
            else
            {
                return RawDataRequest.Cancel();
            }
        }

        public void Update(string message)
        {
            fd.Counter = int.Parse(message);
        }

        public void UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
