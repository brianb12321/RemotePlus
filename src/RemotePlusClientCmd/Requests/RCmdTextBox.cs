using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using ConsoLovers.ConsoleToolkit;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;

namespace RemotePlusClientCmd.Requests
{
    //Interface rcmd_textBox
    public class RCmdTextBox : IDataRequest
    {
        public string URI => "rcmd_textBox";

        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Command line text box";

        string IDataRequest.Description => "Provides a simple command line based text box";

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            return RawDataRequest.Success(new InputBox<string>($"{builder.UnsafeResolve<PromptRequestOptions>().Message}: ").ReadLine());
        }

        void IDataRequest.UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
