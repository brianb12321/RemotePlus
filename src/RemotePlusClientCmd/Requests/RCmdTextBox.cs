using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using ConsoLovers.ConsoleToolkit;

namespace RemotePlusClientCmd.Requests
{
    //Interface rcmd_textBox
    public class RCmdTextBox : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Command line text box";

        string IDataRequest.Description => "Provides a simple command line based text box";

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            return RawDataRequest.Success((string)new InputBox<string>($"{builder.Message}: ").ReadLine());
        }

        void IDataRequest.UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
