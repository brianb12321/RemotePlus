using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Requests
{
    public class RequestStringRequest : StandordRequest<RequestStringRequestBuilder, UpdateRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Request String";

        public override string Description => "Requests the user for a string.";

        public override string URI => "r_string";

        public override NetworkSide SupportedSides => NetworkSide.Client | NetworkSide.Server;

        public override RawDataResponse RequestData(RequestStringRequestBuilder builder, NetworkSide executingSide)
        {
            RequestStringDialogBox rd = new RequestStringDialogBox(builder.Message);
            if (rd.ShowDialog() == DialogResult.OK)
            {
                return RawDataResponse.Success(rd.Data);
            }
            else
            {
                return RawDataResponse.Cancel();
            }
        }
    }
}