using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using Logging;
using System.ServiceModel;
using RemotePlusLibrary.Core;
using System.Windows.Forms;
using System.Drawing;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.ClientModule;

namespace RemotePlusClientCmd
{
    public partial class ClientCmdManager
    {
        public static Dictionary<string, CommandDelegate> LocalCommands = new Dictionary<string, CommandDelegate>();
        public static IRemote Remote = null;
        public static CMDLogging Logger = null;
        public static DuplexChannelFactory<IRemote> channel = null;
        public static bool WaitFlag = true;
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                InitCommands();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Logger = new CMDLogging()
                {
                    DefaultFrom = "CLient CMD",
                    OverrideLogItemObjectColorValue = true
                };
                InitializeDefaultKnownTypes();
                RequestStore.Init();
                Console.Write("Enter url: ");
                string url = Console.ReadLine();
                channel = new DuplexChannelFactory<IRemote>(new ClientCallback(), "DefaultEndpoint");
                channel.Endpoint.Address = new EndpointAddress(url);
                Console.Write("Enter Username: ");
                string username = Console.ReadLine();
                Console.Write("Enter Password: ");
                string password = Console.ReadLine();
                RegistirationObject ro = new RegistirationObject();
                //{
                ro.LoginRightAway = true;
                ro.Credentials = new UserCredentials(username, password);
                //};
                Remote = channel.CreateChannel();
                Remote.Register(ro);
                Console.WriteLine("Enter a command to the server. Type {help} for a list of commands.");
                while (true)
                {
                    if(!WaitFlag)
                    {
                        Console.Write(">");
                        var c = Console.ReadLine();
                        if (c.ToCharArray()[0] == '#')
                        {
                            RunLocalCommand(c);
                        }
                        else
                        {
                            Remote.RunServerCommand(c);
                        }
                    }
                }
#pragma warning disable CS0162 // Unreachable code detected
                channel.Close();
#pragma warning restore CS0162 // Unreachable code detected
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.AddOutput(new LogItem(OutputLevel.Error, "Client error. " + ex.ToString(), "Client") { Color = ConsoleColor.Red });
#else
                Logger.AddOutput(new LogItem(OutputLevel.Error, "Client error. " + ex.Message, "Client") { Color = ConsoleColor.Red });
#endif
            }
        }

        private static void InitCommands()
        {
            LocalCommands.Add("#help", Help);
            LocalCommands.Add("#clear", clearScreen);
            LocalCommands.Add("#close", close);
            LocalCommands.Add("#title", title);
            LocalCommands.Add("#load-commandFile", load_CommandFile);
        }

        static int RunLocalCommand(string command)
        {
            try
            {
                bool FoundCommand = false;
                string[] ca = command.Split();
                foreach (KeyValuePair<string, CommandDelegate> k in LocalCommands)
                {
                    if (ca[0] == k.Key)
                    {
                        FoundCommand = true;
                        return k.Value(ca);
                    }
                }
                if (!FoundCommand)
                {
                    Logger.AddOutput("Unknown local command. Please type {#help} for a list of commands.", OutputLevel.Error);
                    return (int)CommandStatus.Fail;
                }
                return -2;
            }
            catch (Exception ex)
            {
                Logger.AddOutput("Error whie executing local command: " + ex.Message, OutputLevel.Error);
                return (int)CommandStatus.Fail;
            }
        }
        static void InitializeDefaultKnownTypes()
        {
            Logger.AddOutput("Initializing default known types.", OutputLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }
    }
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