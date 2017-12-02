using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;

namespace RemotePlusClientCmd.Requests
{
    //Interface: rcmd_multitextBox
    public class RCmdMultiLineTextbox : IDataRequest
    {
        public bool ShowProperties => false;

        public string FriendlyName => "Commandline Multiline Textbox";

        public string Description => "Allows you to enter multiple lines of text.";

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            StringBuilder sb = new StringBuilder();
            Console.WriteLine($"{builder.Message}");
            while(true)
            {
                string result = Console.ReadLine();
                if(result.Length > 0)
                {
                    sb.AppendLine(result);
                }
                else
                {
                    return RawDataRequest.Success(sb.ToString());
                }
            }
        }

        public void UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
