using RemotePlusLibrary;
using System.ServiceModel;
using System.Windows.Forms;
using Logging;
using System;

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

        public UserCredentials RequestAuthentication(AuthenticationRequest Request)
        {
            AuthenticationDialog ad = new AuthenticationDialog(Request.Reason);
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
            if (MainF.ServerConsoleObj == null)
            {
                li.From = "Server Console";
                MainF.ConsoleObj.Logger.AddOutput(li);
            }
            else
            {
                MainF.ServerConsoleObj.Logger.AddOutput(li);
            }
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder builder = new ClientBuilder();
            builder.FriendlyName = "Default GUI Client";
            return builder;
        }

        public void TellMessageToServerConsole(string Message)
        {
            if (MainF.ServerConsoleObj == null)
            {
                MainF.ConsoleObj.AppendText(Message);
            }
            else
            {
                MainF.ServerConsoleObj.AppendText(Message);
            }
        }
    }
}
