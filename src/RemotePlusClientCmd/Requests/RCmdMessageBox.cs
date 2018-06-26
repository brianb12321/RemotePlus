using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using RemotePlusLibrary.RequestSystem;

namespace RemotePlusClientCmd.Requests
{
    //Interface: rcmd_messageBox
    public class RCmdMessageBox : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Command Line Message Box";

        string IDataRequest.Description => "Displays a message box on the command line.";

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            ConsoLovers.ConsoleToolkit.ConsoleMessageBox cmb = new ConsoLovers.ConsoleToolkit.ConsoleMessageBox();
            var result = cmb.Show(builder.Message);
            return RawDataRequest.Success(result.ToString());
        }

        void IDataRequest.UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
