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
    public class ConsoleMenuRequest : StandordRequest<ConsoleMenuRequestBuilder, UpdateRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Menu Request";

        public override string Description => "Provides a simple menu";

        public override string URI => "rcmd_smenu";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        public override RawDataResponse RequestData(ConsoleMenuRequestBuilder builder, NetworkSide executingSide)
        {
            while(true)
            {
                Console.WriteLine($"{builder.Message}");
                Console.WriteLine("============================");
                foreach (KeyValuePair<string, string> optionsList in builder.MenuItems)
                {
                    Console.WriteLine($"{optionsList.Key}){optionsList.Value}");
                }
                Console.WriteLine();
                Console.Write("Please enter a value: ");
                ConsoleKeyInfo option = Console.ReadKey(false);
                Console.WriteLine();
                if(!builder.MenuItems.ContainsKey(option.KeyChar.ToString()))
                {
                    Console.WriteLine("Please enter a valid option.");
                }
                else
                {
                    return RawDataResponse.Success(option.KeyChar);
                }
            }
        }
    }
}