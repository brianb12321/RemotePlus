using System;
using ConsoLovers.ConsoleToolkit;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;

namespace RemotePlusClientCmd.Requests
{
    public class RCmdTextBox : StandordRequest<RCmdTextBoxBuilder>
    {
        public override string URI => "rcmd_textBox";

        public override bool ShowProperties => false;

        public override string FriendlyName => "Command line text box";

        public override string Description => "Provides a simple command line based text box";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override void Update(string message)
        {
            throw new NotImplementedException();
        }

        public override RawDataResponse RequestData(RCmdTextBoxBuilder builder, NetworkSide executingSide)
        {
            return RawDataResponse.Success(new InputBox<string>($"{builder.Message}: ").ReadLine());
        }
    }
}