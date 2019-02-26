using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClientCmd.Requests
{
    public class ConsoleReadLineRequest : StandordRequest<ConsoleReadLineRequestBuilder, UpdateRequestBuilder>
    {
        public override string URI => "rcmd_readLine";
        public override bool ShowProperties => false;
        public override string FriendlyName => "Console ReadLine Request";
        public override string Description => "What do you think it does?";
        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(ConsoleReadLineRequestBuilder builder, NetworkSide executingSide)
        {
            if(!string.IsNullOrWhiteSpace(builder.Message))
            {
                Console.Write(builder.Message);
            }
            Console.ForegroundColor = builder.LineColor;
            RawDataResponse response = RawDataResponse.Success(ConsoleHelper.LongReadLine());
            Console.ResetColor();
            return response;
        }
    }
}