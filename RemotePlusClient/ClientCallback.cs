using RemotePlusLibrary;
using System.ServiceModel;
using System.Windows.Forms;
using Logging;
using System;
using System.Net;
using System.Drawing;
using RemotePlusClient.CommonUI;

namespace RemotePlusClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false)]
    class ClientCallback : IRemoteClient
    {
        public void Disconnect(string Reason)
        {
            LogItem l = new LogItem(Logging.OutputLevel.Error, "The server disconnected from the client. Reason: " + Reason, "Server Host");
            MainF.ConsoleObj.Logger.AddOutput(l);
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
            ClientApp.Logger.AddOutput(Message, o, "Server Host");
            MainF.ConsoleObj.Logger.AddOutput(Message, o, "Server Host");
        }

        public void TellMessage(UILogItem li)
        {
            ClientApp.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From));
            MainF.ConsoleObj.Logger.AddOutput(li);
        }

        public void TellMessage(UILogItem[] Logs)
        {
            foreach(LogItem li in Logs)
            {
                ClientApp.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From));
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
            ClientBuilder builder = new ClientBuilder()
            {
                FriendlyName = "Default GUI Client"
            };
            builder.ExtraData.Add("global_newLine", "true");
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

        public ReturnData RequestInformation(RequestBuilder builder)
        {
            return RequestDialogBoxStore.Show(builder);
        }
    }
}