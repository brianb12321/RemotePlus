using RemotePlusLibrary.RequestSystem;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Requests
{
    public sealed class ColorRequest : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Color Request";

        string IDataRequest.Description => "Requests a color from the user.";

        string IDataRequest.URI => "r_color";

        void IDataRequest.UpdateProperties()
        {
            throw new NotImplementedException();
        }

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                return RawDataRequest.Success(cd.Color.ToString());
            }
            else
            {
                return RawDataRequest.Cancel(Color.Black.ToString());
            }
        }

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        void IDataRequest.Update(string message)
        {
            throw new NotImplementedException();
        }
    }
}