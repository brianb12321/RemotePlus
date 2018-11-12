using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;

namespace RemotePlusClientCmd.Requests
{
    public class ConsoleMenuRequest : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Menu Request";

        string IDataRequest.Description => "Provides a simple menu";

        string IDataRequest.URI => "rcmd_smenu";

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            var options = builder.UnsafeResolve<SimpleMenuRequestOptions>();
            while(true)
            {
                Console.WriteLine($"{options.Message}");
                Console.WriteLine("============================");
                foreach (KeyValuePair<string, string> optionsList in options.MenuItems)
                {
                    Console.WriteLine($"{optionsList.Key}){optionsList.Value}");
                }
                Console.WriteLine();
                Console.Write("Please enter a value: ");
                ConsoleKeyInfo option = Console.ReadKey(false);
                Console.WriteLine();
                if(!options.MenuItems.ContainsKey(option.KeyChar.ToString()))
                {
                    Console.WriteLine("Please enter a valid option.");
                }
                else
                {
                    return RawDataRequest.Success(option.KeyChar);
                }
            }
        }

        void IDataRequest.Update(string message)
        {
            throw new NotImplementedException();
        }

        void IDataRequest.UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
