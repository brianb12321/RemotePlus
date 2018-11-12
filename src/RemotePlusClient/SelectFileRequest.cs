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

namespace RemotePlusClient
{
    public class SelectFileRequest : IDataRequest
    {
        public bool ShowProperties => false;

        public string FriendlyName => "Select File Data Request";

        public string Description => "Stuff";

        public string URI => "global_selectFile";

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            FindRemoteFileDialog fd = new FindRemoteFileDialog(FilterMode.File, MainF.Remote, MainF.CurrentConnectionData.BaseAddress, MainF.CurrentConnectionData.Port);
            
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
            throw new NotImplementedException();
        }

        public void UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
