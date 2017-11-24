using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;

namespace RemotePlusClientCmd.Requests
{
    public class ConsoleMenuRequest : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Menu Request";

        string IDataRequest.Description => "Provides a simple menu";

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            while(true)
            {
                Console.WriteLine($"{builder.Message}");
                Console.WriteLine("============================");
                foreach (KeyValuePair<string, string> options in builder.Arguments)
                {
                    Console.WriteLine($"{options.Key}){options.Value}");
                }
                Console.WriteLine();
                Console.Write("Please enter a value: ");
                ConsoleKeyInfo option = Console.ReadKey(false);
                Console.WriteLine();
                if(!builder.Arguments.ContainsKey(option.KeyChar.ToString()))
                {
                    Console.WriteLine("Please enter a valid option.");
                }
                else
                {
                    return RawDataRequest.Success(option.KeyChar);
                }
            }
        }

        void IDataRequest.UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
