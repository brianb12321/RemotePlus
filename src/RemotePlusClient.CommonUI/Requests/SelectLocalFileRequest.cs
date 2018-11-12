using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Requests
{
    public class SelectLocalFileRequest : IDataRequest
    {
        public bool ShowProperties => false;

        public string FriendlyName => "Select Local File Request";

        public string Description => "Requests for a local path. A local path is a path on the client.";

        public string URI => "r_selectLocalFile";

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            var options = builder.UnsafeResolve<FileDialogRequestOptions>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = options.Title;
            ofd.Filter = options.Filter;
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                return RawDataRequest.Success(ofd.FileName);
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