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
    //Interface: rcmd_multitextBox
    public class RCmdMultiLineTextbox : StandordRequest<RCmdMultilineRequestBuilder, UpdateRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Commandline Multiline Textbox";

        public override string Description => "Allows you to enter multiple lines of text.";

        public override string URI => "rcmd_mTextBox";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(RCmdMultilineRequestBuilder builder, NetworkSide executingSide)
        {
            StringBuilder sb = new StringBuilder();
            Console.WriteLine($"{builder.Message}");
            while(true)
            {
                string result = Console.ReadLine();
                if(result.Length > 0)
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(result);
                    }
                }
                else
                {
                    return RawDataResponse.Success(sb.ToString());
                }
            }
        }
    }
}