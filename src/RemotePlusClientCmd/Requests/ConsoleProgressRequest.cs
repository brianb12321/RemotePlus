using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using ShellProgressBar;

namespace RemotePlusClientCmd.Requests
{
    public class ConsoleProgressRequest : StandordRequest<ProgressRequestBuilder, ProgressUpdateBuilder>
    {
        public override string URI => "r_progress";
        public override bool ShowProperties => false;
        public override string FriendlyName => "Console Progress Request";
        public override string Description => "Provdes a console implementation for the progress request.";
        public override NetworkSide SupportedSides => NetworkSide.Client;
        private ProgressBar pb = null;
        private ProgressBarOptions options = null;
        public ConsoleProgressRequest()
        {
            options = new ProgressBarOptions();
            options.DisplayTimeInRealTime = false;
            options.ForegroundColor = ConsoleColor.DarkYellow;
            options.ForegroundColorDone = ConsoleColor.Green;
        }
        public override RawDataResponse RequestData(ProgressRequestBuilder builder, NetworkSide executingSide)
        {
            pb = new ProgressBar(builder.Maximum, builder.Message, options);
            return RawDataResponse.Success(null);
        }
        public override void Update(ProgressUpdateBuilder message)
        {
            pb.Tick(message.NewValue, message.Text);
        }
        public override void Dispose()
        {
            pb.Dispose();
        }
    }
}