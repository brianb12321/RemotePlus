using RemotePlusLibrary;
using System.ServiceModel;
using System.Windows.Forms;
using Logging;

namespace RemotePlusClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false)]
    class ClientCallback : IRemoteClient
    {
        public void Disconnect(string Reason)
        {
            MainF.ConsoleObj.Logger.AddOutput("The server disconnected from the client. Reason: " + Reason, Logging.OutputLevel.Error, "Server Host");
            MainF.Disconnect();
        }

        public UserCredentials RequestAuthentication()
        {
            AuthenticationDialog ad = new AuthenticationDialog();
            if(ad.ShowDialog() == DialogResult.OK)
            {
                return ad.UserInfo;
            }
            else
            {
                return null;
            }
        }

        public void TellMessage(string Message, Logging.OutputLevel o)
        {
            MainF.ConsoleObj.Logger.AddOutput(Message, o, "Server Host");
        }

        public void TellMessage(UILogItem li)
        {
            MainF.ConsoleObj.Logger.AddOutput(li);
        }

        public void TellMessage(UILogItem[] Logs)
        {
            foreach(LogItem li in Logs)
            {
                MainF.ConsoleObj.Logger.AddOutput(li);
            }
        }

        public void TellMessageToServerConsole(UILogItem li)
        {
            MainF.ServerConsoleObj.Logger.AddOutput(li);
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder builder = new ClientBuilder();
            builder.FriendlyName = "Default GUI Client";
            return builder;
        }

        public void TellMessageToServerConsole(string Message)
        {
            MainF.ServerConsoleObj.AppendText(Message);
        }
    }
}
