using RemotePlusClient.CommonUI;
using RemotePlusLibrary;
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

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            FindRemoteFileDialog fd = new FindRemoteFileDialog(FilterMode.File, MainF.Remote, MainF.BaseAddress, MainF.Port);
            
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
