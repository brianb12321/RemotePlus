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
    //Interface: rcmd_multitextBox
    public class RCmdMultiLineTextbox : IDataRequest
    {
        public bool ShowProperties => false;

        public string FriendlyName => "Commandline Multiline Textbox";

        public string Description => "Allows you to enter multiple lines of text.";

        public string URI => "rcmd_mTextBox";

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            StringBuilder sb = new StringBuilder();
            Console.WriteLine($"{builder.UnsafeResolve<PromptRequestOptions>().Message}");
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
                    return RawDataRequest.Success(sb.ToString());
                }
            }
        }

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        public void UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
