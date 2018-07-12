using RemotePlusLibrary.RequestSystem;
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

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = builder.Message;
            if (builder.Metadata.ContainsKey("Filter"))
            {
                ofd.Filter = builder.Metadata["Filter"];
            }
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