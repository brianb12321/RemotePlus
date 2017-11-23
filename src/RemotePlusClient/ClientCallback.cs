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
    public class ClientCallback : IRemoteClient
    {
        public bool SwapFlag { get; private set; }
        public bool ConsoleStreamEnabled { get; private set; } = true;
        Logger consoleStream = null;
        /// <summary>
        /// Tells the client which textbox to use for the server output
        /// </summary>
        /// <param name="logger"></param>
        public void SwapConsoleStream(Logger logger)
        {
            consoleStream = logger;
            SwapFlag = true;
        }
        public void DisableConsoleStream()
        {
            ConsoleStreamEnabled = false;
        }
        public void EnableConsoleStream()
        {
            ConsoleStreamEnabled = true;
        }

        #region Callback Methods
        public void Disconnect(string Reason)
        {
            LogItem l = new LogItem(Logging.OutputLevel.Error, "The server disconnected from the client. Reason: " + Reason, "Server Host");
            MainF.ConsoleObj.Logger.AddOutput(l);
            MainF.Disconnect();
        }

        public UserCredentials RequestAuthentication(AuthenticationRequest Request)
        {
            AuthenticationDialog ad = new AuthenticationDialog(Request);
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
            if (ConsoleStreamEnabled)
            {
                if (!SwapFlag)
                {
                    ClientApp.Logger.AddOutput(Message, o, "Server Host");
                }
                else
                {
                    consoleStream.AddOutput(Message, o, "Server Host");
                }
                MainF.ConsoleObj.Logger.AddOutput(Message, o, "Server Host");
            }
        }

        public void TellMessage(UILogItem li)
        {
            if (ConsoleStreamEnabled)
            {
                if (!SwapFlag)
                {
                    ClientApp.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From));
                }
                else
                {
                    consoleStream.AddOutput(new LogItem(li.Level, li.Message, li.From));
                }
                MainF.ConsoleObj.Logger.AddOutput(li);
            }
        }

        public void TellMessage(UILogItem[] Logs)
        {
            if (ConsoleStreamEnabled)
            {
                foreach (LogItem li in Logs)
                {
                    if (!SwapFlag)
                    {
                        ClientApp.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From));
                    }
                    else
                    {
                        consoleStream.AddOutput(new LogItem(li.Level, li.Message, li.From));
                    }
                    MainF.ConsoleObj.Logger.AddOutput(li);
                }
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
            ClientBuilder builder = new ClientBuilder(ClientType.GUI)
            {
                FriendlyName = "Default GUI Client"
            };
            builder.ExtraData.Add("global_newLine", "true");
            builder.ExtraData.Add("ps_appendNewLine", "false");
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
            return RequestStore.Show(builder);
        }

        public void RegistirationComplete()
        {
        }
        #endregion

    }
}