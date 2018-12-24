using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Requests
{
    public sealed class ColorRequest : StandordRequest<RequestBuilder, UpdateRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Color Request";

        public override string Description => "Requests a color from the user.";

        public override string URI => "r_color";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(RequestBuilder builder, NetworkSide executingSide)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                return RawDataResponse.Success(cd.Color.ToString());
            }
            else
            {
                return RawDataResponse.Cancel(Color.Black.ToString());
            }
        }
    }
}