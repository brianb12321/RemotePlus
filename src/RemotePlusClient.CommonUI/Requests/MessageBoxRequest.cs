using RemotePlusLibrary;
using RemotePlusLibrary.RequestSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Requests
{
    public sealed class MessageBoxRequest : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Message Box Request";

        string IDataRequest.Description => "Shows a message box to the user.";

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            try
            {
                var result = MessageBox.Show(builder.Message, builder.Arguments["Caption"], (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), builder.Arguments["Buttons"]), (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), builder.Arguments["Icon"]));
                return RawDataRequest.Success(result.ToString());
            }
            catch
            {
                return RawDataRequest.Cancel();
            }
        }

        void IDataRequest.UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
