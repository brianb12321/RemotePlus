using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI
{
    public sealed class ColorRequest : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Color Request";

        string IDataRequest.Description => "Requests a color from the user.";

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
    }
}