using RemotePlusLibrary;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;
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

        string IDataRequest.URI => "r_messageBox";

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            try
            {
                var options = builder.UnsafeResolve<MessageBoxRequestOptions>();
                var result = MessageBox.Show(options.Message, options.Caption, options.Buttons, options.Icons);
                return RawDataRequest.Success(result.ToString());
            }
            catch
            {
                return RawDataRequest.Cancel();
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