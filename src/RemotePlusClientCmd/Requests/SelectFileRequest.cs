using RemotePlusClient.CommonUI;
using RemotePlusLibrary;
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
            fd = new FindRemoteFileDialog(FilterMode.File, ClientCmdManager.Remote, ClientCmdManager.BaseURL, ClientCmdManager.Port);
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
