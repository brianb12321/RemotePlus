using RemotePlusLibrary;
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
    public sealed class MessageBoxRequest : StandordRequest<MessageBoxRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Message Box Request";

        public override string Description => "Shows a message box to the user.";

        public override string URI => "r_messageBox";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(MessageBoxRequestBuilder builder, NetworkSide executingSide)
        {
            try
            {
                var result = MessageBox.Show(builder.Message, builder.Caption, builder.Buttons, builder.Icons);
                return RawDataResponse.Success(result.ToString());
            }
            catch
            {
                return RawDataResponse.Cancel();
            }
        }

        public override void Update(string message)
        {
            throw new NotImplementedException();
        }
    }
}