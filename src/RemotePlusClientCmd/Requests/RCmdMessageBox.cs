using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;

namespace RemotePlusClientCmd.Requests
{
    //Interface: rcmd_messageBox
    public class RCmdMessageBox : StandordRequest<RCmdMessageBoxBuilder, UpdateRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Command Line Message Box";

        public override string Description => "Displays a message box on the command line.";

        public override string URI => "rcmd_messageBox";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(RCmdMessageBoxBuilder builder, NetworkSide executingSide)
        {
            ConsoLovers.ConsoleToolkit.ConsoleMessageBox cmb = new ConsoLovers.ConsoleToolkit.ConsoleMessageBox();
            var result = cmb.Show(builder.Message);
            return RawDataResponse.Success(result.ToString());
        }
    }
}