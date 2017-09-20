using System;
using RemotePlusLibrary;
using Logging;
using System.ServiceModel;
using RemotePlusClient.CommonUI;

namespace RemotePlusClientCmd
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single,
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false)]
    class ClientCallback : IRemoteClient
    {
        public void Disconnect(string Reason)
        {
            ClientCmdManager.channel.Close();
            ClientCmdManager.Logger.AddOutput(new LogItem(OutputLevel.Error, "Server disconnected. " + Reason, "CLient") { Color = ConsoleColor.Red });
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder cb = new ClientBuilder(ClientType.CommandLine);
            cb.FriendlyName = "RemotePlus Client Command Line";
            cb.ExtraData.Add("ps_appendNewLine", "false");
            return cb;
        }

        public void RegistirationComplete()
        {
            ClientCmdManager.WaitFlag = false;
        }

        public UserCredentials RequestAuthentication(AuthenticationRequest Request)
        {
            Console.WriteLine($"The server requires authentication. Reason: {Request.Reason}");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            return new UserCredentials(username, password);
        }

        public ReturnData RequestInformation(RequestBuilder builder)
        {
            return RequestStore.Show(builder);
        }

        public void TellMessage(string Message, OutputLevel o)
        {
            LogItem li = new LogItem(o, Message, "Server Host");
            if (o == OutputLevel.Warning)
            {
                li.Color = ClientCmdManager.Logger.ConsoleForegroundWarning;
            }
            else if (o == OutputLevel.Info)
            {
                li.Color = ClientCmdManager.Logger.ConsoleForegroundInfo;
            }
            else if (o == OutputLevel.Error)
            {
                li.Color = ClientCmdManager.Logger.ConsoleForegroundError;
            }
            else if(o == OutputLevel.Debug)
            {
                li.Color = ClientCmdManager.Logger.ConsoleForegroundDebug;
            }
            ClientCmdManager.Logger.AddOutput(li);
        }

        public void TellMessage(UILogItem li)
        {
            if (ClientCmdManager.Logger.OverrideLogItemObjectColorValue)
            {
                if (li.Level == OutputLevel.Warning)
                {
                    li.Color = ClientCmdManager.Logger.ConsoleForegroundWarning;
                }
                else if(li.Level == OutputLevel.Info)
                {
                    li.Color = ClientCmdManager.Logger.ConsoleForegroundInfo;
                }
                else if (li.Level == OutputLevel.Error)
                {
                    li.Color = ClientCmdManager.Logger.ConsoleForegroundError;
                }
                else if(li.Level == OutputLevel.Debug)
                {
                    li.Color = ClientCmdManager.Logger.ConsoleForegroundDebug;
                }
            }
            ClientCmdManager.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From) { Color = li.Color });
        }

        public void TellMessage(UILogItem[] Logs)
        {
            foreach (UILogItem l in Logs)
            {
                if (ClientCmdManager.Logger.OverrideLogItemObjectColorValue)
                {
                    if (l.Level == OutputLevel.Warning)
                    {
                        l.Color = ClientCmdManager.Logger.ConsoleForegroundWarning;
                    }
                    else if (l.Level == OutputLevel.Info)
                    {
                        l.Color = ClientCmdManager.Logger.ConsoleForegroundInfo;
                    }
                    else if (l.Level == OutputLevel.Error)
                    {
                        l.Color = ClientCmdManager.Logger.ConsoleForegroundError;
                    }
                    else if(l.Level == OutputLevel.Debug)
                    {
                        l.Color = ClientCmdManager.Logger.ConsoleForegroundDebug;
                    }
                }
                ClientCmdManager.Logger.AddOutput(new LogItem(l.Level, l.Message, l.From) { Color = l.Color });
            }
        }

        public void TellMessageToServerConsole(UILogItem li)
        {
            string f = "";
            if (string.IsNullOrEmpty(li.From))
            {
                f = "Server Console " + "Server Host";
            }
            else
            {
                f = "Server Console " + li.From;
            }
            li.From = f;
            if (ClientCmdManager.Logger.OverrideLogItemObjectColorValue)
            {
                if (li.Level == OutputLevel.Warning)
                {
                    li.Color = ClientCmdManager.Logger.ConsoleForegroundWarning;
                }
                else if (li.Level == OutputLevel.Info)
                {
                    li.Color = ClientCmdManager.Logger.ConsoleForegroundInfo;
                }
                else if (li.Level == OutputLevel.Error)
                {
                    li.Color = ClientCmdManager.Logger.ConsoleForegroundError;
                }
                else if(li.Level == OutputLevel.Debug)
                {
                    li.Color = ClientCmdManager.Logger.ConsoleForegroundDebug;
                }
            }
            ClientCmdManager.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From) { Color = li.Color });
        }

        public void TellMessageToServerConsole(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}